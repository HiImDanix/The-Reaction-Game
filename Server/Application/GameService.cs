using Application.Errors;
using Application.Gaming;
using Domain;
using Domain.Constants;
using Domain.MiniGames;
using FluentResults;
using Hangfire;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enums;

namespace Application;

public class GameService: IGameService
{
    private readonly Repository _context;
    private readonly ILogger<GameService> _logger;

    public GameService(Repository context, ILogger<GameService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result> StartGame(string roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.CurrentGame).ThenInclude(game => game.MiniGames)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        
        if (room == null)
        {
            _logger.LogError("Room with id: {RoomId} was not found", roomId);
            return Result.Fail(new NotFoundError($"Room with id {roomId} was not found"));
        }
        
        // TODO: add back
        // if (room.CurrentGame.Status != Game.GameStatus.Lobby)
        // {
        //     _logger.LogError("CurrentGame has already started");
        //     return Result.Fail(new BusinessValidationError("CurrentGame has already started"));
        // }

        // Add ColorTap mini game. Later, the host will be able to choose the mini game
        _logger.LogInformation("Adding ColorTap mini game to the game");
        var colorTapMiniGame = MiniGameFactory.CreateMiniGame(
            MiniGameType.ColorTap,
            2, 
            ColorTapConstants.RoundDuration);
        room.CurrentGame.MiniGames.Add(colorTapMiniGame);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("CurrentGame engine started for room: {RoomId} in the background", roomId);
        BackgroundJob.Enqueue<GameEngine>(engine => engine.StartGame(room.Id));
        
        return Result.Ok();
    }
}
