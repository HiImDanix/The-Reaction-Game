using Application.Errors;
using AutoMapper;
using Contracts.Output;
using Contracts.Output.Hub;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.MiniGames;
using FluentResults;
using Infrastructure;

namespace Application;

public class RoomService : IRoomService
{
    private readonly Repository _context;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly ILobbyHub _lobbyHub;

    public RoomService(IMapper mapper, Repository context, IAuthService authService, ILobbyHub lobbyHub)
    {
        _mapper = mapper;
        _context = context;
        _authService = authService;
        _lobbyHub = lobbyHub;
    }

    public async Task<Result<RoomCreatedPersonalResp>> CreateRoomAsync(string hostName)
    {
        var player = new Player(hostName);
        var room = new Room(player);

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Players.Add(player);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            await _context.Entry(room).Collection(r => r.Players).LoadAsync();
            await transaction.CommitAsync();
            
            var token = _authService.GenerateToken(player.Id, player.Name, room.Id);
            var dto = _mapper.Map<RoomCreatedPersonalResp>((room, player, token));
            return Result.Ok(dto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    private async Task<Room?> GetRoomEntityByCodeAsync(string code)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.CurrentGame)
            .ThenInclude(g => g.CurrentMiniGame)
            .FirstOrDefaultAsync(r => r.Code == code);
        return room;
    }
    
    public async Task<Result<RoomResp>> GetRoomById(string roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.CurrentGame)
            .ThenInclude(g => g.CurrentMiniGame)
            .ThenInclude(mg => mg.CurrentRound)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            return Result.Fail(new NotFoundError($"Room with id {roomId} was not found"));
        }
        
        // TODO: Find a better way
        if (room.CurrentGame?.CurrentMiniGame?.CurrentRound is ColorTapRound colorTapRound)
        {
            await _context.Entry(colorTapRound)
                .Collection(r => r.ColorWordPairs)
                .LoadAsync();
        }
        
        var dto = _mapper.Map<RoomResp>(room);
        
        // Debugging
        Console.WriteLine($"CurrentMiniGame: {dto.CurrentGame?.CurrentMiniGame != null}");
        Console.WriteLine($"CurrentRound: {dto.CurrentGame?.CurrentMiniGame?.CurrentRound != null}");
        return Result.Ok(dto);
    }

    public async Task<Result<bool>> IsRoomJoinable(string code)
    {
        var room = await GetRoomEntityByCodeAsync(code);
        if (room == null)
        {
            return Result.Fail(new NotFoundError($"Room with code {code} was not found"));
        } 
        return Result.Ok(room.CurrentGame.Status == Game.GameStatus.Lobby);
    }

    public async Task<Result<RoomJoinedPersonalResp>> JoinRoomAsync(string code, string playerName)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var room = await GetRoomEntityByCodeAsync(code);
            if (room == null)
            {
                return Result.Fail(new NotFoundError("Room with the specified code was not found"));
            }
            
            if (room.CurrentGame.Status != Game.GameStatus.Lobby)
            {
                return Result.Fail(new BusinessValidationError("The room is already in-game. Cannot join"));
            }
            
            var player = new Player(playerName);
            room.Players.Add(player);
        
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            await _context.Entry(room).Collection(r => r.Players).LoadAsync();
            await transaction.CommitAsync();

            var token = _authService.GenerateToken(player.Id, player.Name, room.Id);
            
            var hubDto = _mapper.Map<PlayerJoinedMessage>(player);
            await _lobbyHub.NotifyPlayerJoined(room.Id, hubDto);
            var dtoPersonal = _mapper.Map<RoomJoinedPersonalResp>((room, player, token));
            return Result.Ok(dtoPersonal);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}