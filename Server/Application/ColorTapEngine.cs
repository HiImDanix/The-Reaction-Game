using Domain;
using Domain.Constants;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application;

public class ColorTapEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;

    public ColorTapEngine(Room room, Repository context, ILobbyHub lobbyHub)
    {
        room = room;
        _context = context;
        _lobbyHub = lobbyHub;
    }

    public async Task StartAsync(Room room)
    {
        // _logger.LogInformation("Color Tap game execution started");
        var miniGame = room.Game.CurrentMiniGame;
        var colorTapGame = miniGame as ColorTapGame;
        if (colorTapGame == null)
        {
            throw new InvalidOperationException("Current mini game is not Color Tap");
        }

        // _logger.LogInformation("Color Tap game started");
        // await Task.Delay(ColorTapConstants.RoundDuration, stoppingToken);

        // _logger.LogInformation("Color Tap game finished");
        room.Game.Status = Game.GameStatus.Finished;
        await _context.SaveChangesAsync();
    }
}