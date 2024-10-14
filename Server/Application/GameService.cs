using Application.Errors;
using Application.Gameplay;
using Domain;
using Domain.Constants;
using Domain.Errors;
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
            return Result.Fail(new NotFoundError(ErrorCodes.RoomNotFound));
        }
        
        if (!CanStartGame(room.CurrentGame))
        {
            return Result.Fail(new BusinessValidationError(ErrorCodes.GameCannotBeStarted));
        }

        await AddDefaultMiniGamesAsync(room);
        BackgroundJob.Enqueue<GameEngine>(engine => engine.PlayGameAsync(room.Id));
        return Result.Ok();
    }
    
    private static bool CanStartGame(Game game) => game.Status == Game.GameStatus.Lobby;
    
    private async Task AddDefaultMiniGamesAsync(Room room)
    {
        var colorTapMiniGame = MiniGameFactory.CreateMiniGame(
            MiniGameType.ColorTap,
            ColorTapConstants.DefaultRoundsCount, 
            ColorTapConstants.DefaultRoundDuration);
        room.CurrentGame.MiniGames.Add(colorTapMiniGame);
        await _context.SaveChangesAsync();
    }
}
