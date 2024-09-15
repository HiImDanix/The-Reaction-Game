using Application.Gaming;
using AutoMapper;
using Contracts.Output;
using Domain;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public class GameEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<GameEngine> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public GameEngine(Repository context, ILobbyHub lobbyHub, ILogger<GameEngine> logger, IServiceProvider serviceProvider, IMapper mapper)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public async Task StartGame(string roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.CurrentGame)
            .ThenInclude(game => game.MiniGames)
            .FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new InvalidOperationException($"Room with id {roomId} was not found");
        }
        
        if (room == null)
        {
            throw new InvalidOperationException("Room is not set");
        }
        room.CurrentGame.StartClickedAt = DateTime.UtcNow;

        _logger.LogInformation("'Prepare to start' phase");
        await ChangeGameStatus(room, Game.GameStatus.PrepareToStart);
        await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        await Task.Delay(room.CurrentGame.PreparationDuration);

        _logger.LogInformation("Preparation finished. Starting mini games");
        await ChangeGameStatus(room, Game.GameStatus.InProgress);
        
        foreach (var miniGame in room.CurrentGame.MiniGames)
        {

            // Mini-game: show instructions
            _logger.LogInformation("Mini game phase 1: show instructions");
            room.CurrentGame.CurrentMiniGame = miniGame;
            miniGame.InstructionsStartTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
            await Task.Delay(miniGame.InstructionsDuration);

            // Mini-game: gameplay
            await PlayMiniGame(room, miniGame);
            
            // Mini-game: show results
            _logger.LogInformation("Mini game phase 3: show results");
            room.CurrentGame.CurrentMiniGame = null;
            
            await _context.SaveChangesAsync();
            await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        }
        
        room.CurrentGame.Status = Game.GameStatus.FinalScoreboard;
        await _context.SaveChangesAsync();
    }

    private async Task ChangeGameStatus(Room room, Game.GameStatus status)
    {
        room.CurrentGame.Status = status;
        _context.Entry(room).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    private async Task PlayMiniGame(Room room, MiniGame miniGame)
    {
        _logger.LogInformation("Mini game phase 2: gameplay");

        while (miniGame.CurrentRoundNo <= miniGame.TotalRoundsNo)
        {
            var round = miniGame.CreateRound(DateTime.UtcNow, DateTime.UtcNow.Add(miniGame.RoundDuration));
            miniGame.Rounds.Add(round);
            miniGame.CurrentRound = round;
            round.StartTime = DateTime.UtcNow;
            round.EndTime = round.StartTime.Add(miniGame.RoundDuration);
            miniGame.CurrentRoundNo++;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Playing round {MiniGameCurrentRound} of {MiniGameRoundCount}",
                miniGame.CurrentRoundNo, miniGame.TotalRoundsNo);

            switch (miniGame)
            {
                case ColorTapGame:
                    await _serviceProvider.GetRequiredService<IColorTapEngine>().PlayCurrentRound(room, miniGame);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        _logger.LogInformation("Mini game phase 2: gameplay finished");
    }
}