using Application.Gaming;
using Domain;
using Domain.Constants;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application;

public class ColorTapEngine: IColorTapEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<ColorTapEngine> _logger;

    public ColorTapEngine( Repository context, ILobbyHub lobbyHub, ILogger<ColorTapEngine> logger)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
    }

    public async Task PlayRound(Room room, MiniGame miniGame)
    {
        _logger.LogInformation("Color Tap game execution started");
        var colorTapGame = miniGame as ColorTapGame;
        if (colorTapGame == null)
        {
            throw new InvalidOperationException("Current mini game is not Color Tap");
        }

        _logger.LogInformation("Color Tap game started");
        await Task.Delay(ColorTapConstants.RoundDuration);
        _logger.LogInformation("Color Tap game finished");
    }
}