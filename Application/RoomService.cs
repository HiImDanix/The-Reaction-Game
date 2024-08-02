using Application.Errors;
using AutoMapper;
using Contracts.Output;
using Microsoft.EntityFrameworkCore;
using Domain;
using FluentResults;
using DbContext = Infrastructure.DbContext;

namespace Application;

public class RoomService : IRoomService
{
    private readonly DbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public RoomService(IMapper mapper, DbContext context, IAuthService authService)
    {
        _mapper = mapper;
        _context = context;
        _authService = authService;
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
            
            var token = _authService.GenerateToken(player.Id, room.Id, player.Name);
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
        var room = await _context.Rooms.Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.Code == code);
        return room;
    }
    
    public async Task<Result<RoomResp>> GetRoomById(string roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            return Result.Fail(new NotFoundError($"Room with id {roomId} was not found"));
        }
        
        var dto = _mapper.Map<RoomResp>(room);
        return Result.Ok(dto);
    }

    public async Task<Result<bool>> IsRoomJoinable(string code)
    {
        var room = await GetRoomEntityByCodeAsync(code);
        if (room == null)
        {
            return Result.Fail(new NotFoundError($"Room with code {code} was not found"));
        } 
        return Result.Ok(room.Status == Room.RoomStatus.Lobby);
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
            
            if (room.Status != Room.RoomStatus.Lobby)
            {
                return Result.Fail(new BusinessValidationError("The room is already in progress"));
            }
            
            var player = new Player(playerName);
            room.Players.Add(player);
        
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            await _context.Entry(room).Collection(r => r.Players).LoadAsync();
            await transaction.CommitAsync();

            var token = _authService.GenerateToken(player.Id, room.Id, player.Name);
            var dto = _mapper.Map<RoomJoinedPersonalResp>((room, player, token));
            return Result.Ok(dto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}