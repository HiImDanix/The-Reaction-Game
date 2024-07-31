using Application.Errors;
using AutoMapper;
using Contracts.Output;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain;
using FluentResults;

namespace Application;

public class RoomService : IRoomService
{
    private readonly RoomDbContext _context;
    private readonly IMapper _mapper;

    public RoomService(IMapper mapper, RoomDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<Result<RoomOut>> CreateRoomAsync(string hostName)
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
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var dto = _mapper.Map<RoomOut>((room, player));
        return Result.Ok(dto);
    }
    
    private async Task<Room?> GetRoomEntityByCodeAsync(string code)
    {
        var room = await _context.Rooms.Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.Code == code);
        return room;
    }
    
    public async Task<Result<RoomOut>> GetRoomByPlayerSessionAsync(string sessionToken)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.SessionToken == sessionToken);
        if (player == null)
        {
            return Result.Fail(new NotFoundError("Player with the specified session token was not found"));
        }

        var room = await _context.Rooms.Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.Players.Contains(player));
        
        var dto = _mapper.Map<RoomOut>((room, player));
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

    public async Task<Result<RoomOut>> JoinRoomAsync(string code, string playerName)
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
            
            var dto = _mapper.Map<RoomOut>((room, player));
            return Result.Ok(dto);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}