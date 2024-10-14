using AutoMapper;
using Contracts.Output;
using Contracts.Output.MiniGames;
using Domain;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Gaming;

public class GameEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<GameEngine> _logger;
    private readonly IMapper _mapper;
    private readonly IScoringSystem _scoringSystem;
    private readonly GameEngineConfig _config;
    private readonly IMiniGameEngineFactory _miniGameEngineFactory;

    public GameEngine(Repository context, ILobbyHub lobbyHub, ILogger<GameEngine> logger,
        IMapper mapper, IScoringSystem scoringSystem, IOptions<GameEngineConfig> config,
        IMiniGameEngineFactory miniGameEngineFactory)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
        _mapper = mapper;
        _scoringSystem = scoringSystem;
        _config = config.Value;
        _miniGameEngineFactory = miniGameEngineFactory;
    }

    public async Task PlayGameAsync(string roomId)
    {
        _logger.LogInformation("Starting game for room {RoomId}", roomId);
        var room = await _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.CurrentGame)
            .ThenInclude(game => game.MiniGames)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null)
        {
            throw new InvalidOperationException($"Room with id {roomId} was not found");
        }

        await PrepareThePlayerBeforePlayingAsync(room);
        await PlayAllMiniGamesAsync(room);
        await ShowFinalScoreboardAsync(room);
        _logger.LogInformation("Game completed for room {RoomId}", roomId);
    }

    private async Task PrepareThePlayerBeforePlayingAsync(Room room)
    {
        _logger.LogInformation("Phase 1: Prepare to start countdown for room {RoomId}", room.Id);
        room.CurrentGame.StartClickedAt = DateTime.UtcNow;
        room.CurrentGame.Status = Game.GameStatus.PrepareToStart;
        await _context.SaveChangesAsync();
        await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        await Task.Delay(room.CurrentGame.PreparationDuration);
        _logger.LogDebug("Preparation phase completed for room {RoomId}", room.Id);
    }

    private async Task PlayAllMiniGamesAsync(Room room)
    {
        _logger.LogInformation("Phase 2: Gameplay started for room {RoomId}", room.Id);
        room.CurrentGame.Status = Game.GameStatus.InProgress;
        foreach (var miniGame in room.CurrentGame.MiniGames)
        {
            _logger.LogInformation("Starting mini-game {MiniGameType} for room {RoomId}", miniGame.GetType().Name, room.Id);
            await ShowMiniGameInstructionsAsync(room, miniGame);
            await PlayMiniGameRoundsAsync(room, miniGame);
            _logger.LogInformation("Completed mini-game {MiniGameType} for room {RoomId}", miniGame.GetType().Name, room.Id);
        }
        room.CurrentGame.CurrentMiniGame = null;
        await _context.SaveChangesAsync();
        await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        _logger.LogInformation("All mini-games completed for room {RoomId}", room.Id);
    }

    private async Task ShowMiniGameInstructionsAsync(Room room, MiniGame miniGame)
    {
        _logger.LogInformation("Showing instructions for {MiniGameType} in room {RoomId}", miniGame.GetType().Name, room.Id);
        room.CurrentGame.CurrentMiniGame = miniGame;
        miniGame.InstructionsStartTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        await Task.Delay(miniGame.InstructionsDuration);
        _logger.LogDebug("Instructions shown for {MiniGameType} in room {RoomId}", miniGame.GetType().Name, room.Id);
    }

    private async Task ShowFinalScoreboardAsync(Room room)
    {
        _logger.LogInformation("Phase 3: Showing final scoreboard for room {RoomId}", room.Id);
        room.CurrentGame.Status = Game.GameStatus.FinalScoreboard;
        await _context.SaveChangesAsync();
        await _lobbyHub.NotifyCurrentGameUpdated(room.Id, _mapper.Map<GameResp>(room.CurrentGame));
        _logger.LogDebug("Final scoreboard displayed for room {RoomId}", room.Id);
    }

    private async Task PlayMiniGameRoundsAsync(Room room, MiniGame miniGame)
    {
        _logger.LogInformation("Starting gameplay for {MiniGameType} in room {RoomId}", miniGame.GetType().Name, room.Id);
        var engine = _miniGameEngineFactory.Create(miniGame);

        while (miniGame.CurrentRoundNo < miniGame.TotalRoundsNo)
        {
            await PlaySingleRoundAsync(room, miniGame, engine);
        }

        _logger.LogInformation("Completed all rounds for {MiniGameType} in room {RoomId}", miniGame.GetType().Name, room.Id);
    }

    private async Task PlaySingleRoundAsync(Room room, MiniGame miniGame, IMiniGameEngine engine)
    {
        _logger.LogInformation("Starting round {CurrentRound}/{TotalRounds} of {MiniGameType} in room {RoomId}", 
            miniGame.CurrentRoundNo + 1, miniGame.TotalRoundsNo, miniGame.GetType().Name, room.Id);
        
        await PrepareRoundAsync(miniGame);
        await engine.PlayCurrentRoundAsync(room, miniGame);
        await CalculateAndDisplayRoundResultsAsync(room, miniGame, engine, miniGame.CurrentRound!);
        
        _logger.LogDebug("Completed round {CurrentRound}/{TotalRounds} of {MiniGameType} in room {RoomId}", 
            miniGame.CurrentRoundNo, miniGame.TotalRoundsNo, miniGame.GetType().Name, room.Id);
    }

    private async Task PrepareRoundAsync(MiniGame miniGame)
    {
        _logger.LogDebug("Preparing round for {MiniGameType}", miniGame.GetType().Name);
        var round = miniGame.CreateRound(DateTime.UtcNow, DateTime.UtcNow.Add(miniGame.RoundDuration));
        miniGame.Rounds.Add(round);
        miniGame.CurrentRound = round;
        round.StartTime = DateTime.UtcNow;
        round.EndTime = round.StartTime.Add(miniGame.RoundDuration);
        miniGame.CurrentRoundNo++;
        await _context.SaveChangesAsync();
    }

    private async Task CalculateAndDisplayRoundResultsAsync(Room room, MiniGame miniGame, IMiniGameEngine engine, MiniGameRound round)
    {
        _logger.LogInformation("Calculating scores for round {CurrentRound} of {MiniGameType} in room {RoomId}", 
            miniGame.CurrentRoundNo, miniGame.GetType().Name, room.Id);
        
        var playerMetrics = await engine.CalculateRoundMetrics(room, round);
        var roundResults = _scoringSystem.CalculateRoundScores(playerMetrics);
        round.Scoreboard = roundResults.OrderByDescending(p => p.Score).ToList();
        round.EndTime = DateTime.UtcNow;

        _logger.LogInformation("Displaying scoreboard for round {CurrentRound} of {MiniGameType} in room {RoomId} for {Duration}ms", 
            miniGame.CurrentRoundNo, miniGame.GetType().Name, room.Id, _config.RoundEndScoreboardScreenDuration.TotalMilliseconds);
        
        await _lobbyHub.NotifyCurrentRoundUpdated(room.Id, _mapper.Map<MiniGameRoundResp>(miniGame.CurrentRound));
        await Task.Delay(_config.RoundEndScoreboardScreenDuration);
        
        _logger.LogDebug("Round results displayed for round {CurrentRound} of {MiniGameType} in room {RoomId}", 
            miniGame.CurrentRoundNo, miniGame.GetType().Name, room.Id);
    }
}