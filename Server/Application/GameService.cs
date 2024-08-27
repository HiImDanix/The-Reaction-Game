using Application.Errors;
using Domain;
using Domain.MiniGames;
using FluentResults;
using Hangfire;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public class GameService: IGameService
{
    private readonly Repository _context;
    private readonly IServiceProvider _serviceProvider;
    
    public GameService(Repository context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<Result> StartGame(string roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.Game).ThenInclude(game => game.MiniGames)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        
        if (room == null)
        {
            return Result.Fail(new NotFoundError($"Room with id {roomId} was not found"));
        }
        // if (room.Game.Status != Game.GameStatus.NotStarted)
        // {
        //     return Result.Fail(new BusinessValidationError("Something went wrong. Game is not in 'not started' state"));
        // }
        // Inject a mini game for testing
        var miniGame = new ColorTapGame(2, TimeSpan.FromSeconds(5));
        room.Game.MiniGames.Add(miniGame);
        _context.Entry(room.Game).State = EntityState.Modified;
        if (room.Game.MiniGames.Count < 1)
        {
            return Result.Fail(new BusinessValidationError("No mini games have been chosen"));
        }
        await _context.SaveChangesAsync();
        
        // Re retrieve room for debug
        var room2 = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.Game)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        
        var lobbyHub = _serviceProvider.GetRequiredService<ILobbyHub>();
        var logger = _serviceProvider.GetRequiredService<ILogger<GameEngine>>();
        
         var jobId = BackgroundJob.Enqueue<GameEngine>(engine => engine.StartGame(room.Id));
        
        return Result.Ok();
    }
}
