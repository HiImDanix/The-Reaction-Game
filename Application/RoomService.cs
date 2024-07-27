using AutoMapper;
using Contracts.Output;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain;

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

    public async Task<RoomOut> CreateRoomAsync(string hostName)
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

        return _mapper.Map<RoomOut>((room, player));
    }

    public async Task<RoomOut> GetRoomByCodeAsync(string code)
    {
        var room = await GetRoomEntityByCodeAsync(code);
        return _mapper.Map<RoomOut>(room);
    }
    
    private async Task<Room> GetRoomEntityByCodeAsync(string code)
    {
        var room = await _context.Rooms.Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.Code == code);
        
        return room ?? throw new NotFoundException($"Room with code '{code}' not found.");
    }
    
    public async Task<RoomOut> GetRoomByYourPlayerSessionAsync(string sessionToken)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.SessionToken == sessionToken);
        if (player == null)
        {
            throw new NotFoundException("Player not found.");
        }

        var room = await _context.Rooms.Include(r => r.Players)
            .FirstOrDefaultAsync(r => r.Players.Contains(player));
        
        return _mapper.Map<RoomOut>((room, player));
    }
    
    public async Task<RoomOut> JoinRoomAsync(string code, string playerName)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var room = await GetRoomEntityByCodeAsync(code);
            
            if (room.Status != Room.RoomStatus.Lobby)
            {
                throw new CannotJoinRoomException("Room is not in lobby state.");
            }
            
            var player = new Player(playerName);
            room.Players.Add(player);
        
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            await _context.Entry(room).Collection(r => r.Players).LoadAsync();
            await transaction.CommitAsync();
            
            return _mapper.Map<RoomOut>((room, player));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
    
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class CannotJoinRoomException : Exception
{
    public CannotJoinRoomException(string message) : base(message)
    {
    }
}