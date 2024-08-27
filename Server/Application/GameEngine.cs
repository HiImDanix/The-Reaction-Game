using Domain;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class GameEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<GameEngine> _logger;

    public GameEngine(Repository context, ILobbyHub lobbyHub, ILogger<GameEngine> logger)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
    }

    public async Task StartGame(string roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.Game)
            .ThenInclude(game => game.MiniGames)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new InvalidOperationException($"Room with id {roomId} was not found");
        }
        await ExecuteAsync(room);
    }

    private async Task ExecuteAsync(Room room)
    {
        _logger.LogInformation("GameEngine execution started");
        if (room == null)
        {
            throw new InvalidOperationException("Room is not set");
        }

        _logger.LogInformation("Game preparation started");
        await ChangeGameStatus(room, Game.GameStatus.PrepareToStart);
        await _lobbyHub.NotifyGameStatusChanged(room.Id, Game.GameStatus.PrepareToStart);
        // await Task.Delay(_room.Game.PreparationTime, stoppingToken);
        _logger.LogInformation("Game preparation finished");

        _logger.LogInformation("Game started");
        await ChangeGameStatus(room, Game.GameStatus.InProgress);
        await _lobbyHub.NotifyGameStatusChanged(room.Id, Game.GameStatus.InProgress);
        await StartGame(room);
    }

    private async Task StartGame(Room room)
    {
        var round = 1;
        foreach (var miniGame in room.Game.MiniGames)
        {

            // Mini game starting: show instructions
            _logger.LogInformation("Mini game started: show instructions");
            room.Game.CurrentMiniGame = miniGame;
            miniGame.InstructionStartTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _lobbyHub.NotifyMiniGameStartedShowInstructions(room.Id, miniGame.Name, miniGame.Instructions, miniGame.InstructionsDuration);
            // await Task.Delay(miniGame.InstructionsDuration, stoppingToken);

            // Mini game starting: show game
            miniGame.CurrentRound = round++;
            await PlayMiniGame(room);
        }

    }

    private async Task ChangeGameStatus(Room room, Game.GameStatus status)
    {
        room.Game.Status = status;
        await _context.SaveChangesAsync();
    }

    private async Task PlayMiniGame(Room room)
    {
        _logger.LogInformation("Mini game started: show game");
        var miniGame = room.Game.CurrentMiniGame;
        
        switch (miniGame)
        {
            case ColorTapGame :
                await new ColorTapEngine(room, _context, _lobbyHub).StartAsync(room);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}