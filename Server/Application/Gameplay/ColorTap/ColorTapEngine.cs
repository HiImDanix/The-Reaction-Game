using System.Drawing;
using AutoMapper;
using Contracts.Output;
using Domain;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Gameplay.ColorTap;

public class ColorTapEngine : IColorTapEngine
{
    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<ColorTapEngine> _logger;
    private readonly IMapper _mapper;
    private readonly ColorTapConfig _config;

    public ColorTapEngine(Repository context, ILobbyHub lobbyHub, ILogger<ColorTapEngine> logger, IMapper mapper, IOptions<ColorTapConfig> config)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
        _mapper = mapper;
        _config = config.Value;
    }

    public async Task PlayCurrentRoundAsync(Room room, MiniGame miniGame)
    {
        try
        {
            if (miniGame is not ColorTapGame)
            {
                throw new InvalidOperationException("Mini game is not of type ColorTapGame");
            }
        
            if (miniGame.CurrentRound is not ColorTapRound round)
            {
                throw new InvalidOperationException("Current round is not of type ColorTapRound");
            }
        
            _logger.LogInformation("Color Tap game round {RoundNo} started", miniGame.CurrentRoundNo);
        
            // Generate data for the round and notify the clients
            round.ColorWordPairs = GenerateColorWordPairs(round.StartTime, round.EndTime);
            _context.ColorTapRounds.Update(round);
            await _context.SaveChangesAsync();
            await _lobbyHub.NotifyCurrentMiniGameUpdated(room.Id, _mapper.Map<MiniGameResp>(miniGame));
            _logger.LogDebug("Generated {PairCount} color-word pairs for round {RoundNo}", 
                round.ColorWordPairs.Count, miniGame.CurrentRoundNo);
        
            _logger.LogDebug("Waiting for round {RoundNo} to finish. Duration: {RoundDuration}", 
                miniGame.CurrentRoundNo, miniGame.RoundDuration);
            await Task.Delay(miniGame.RoundDuration);
            _logger.LogDebug("Color Tap round {RoundNo} finished", miniGame.CurrentRoundNo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred during Color Tap game round {RoundNo}", miniGame.CurrentRoundNo);
            throw;
        }
    }

    public Task<IEnumerable<PlayerMetrics>> CalculateRoundMetrics(Room room, MiniGameRound miniGameRound)
    {
        if (miniGameRound is not ColorTapRound round)
        {
            throw new InvalidOperationException("Mini game round is not of type ColorTapRound");
        }
        
        var players = room.Players;
        
        // TODO: Implement the logic to calculate player metrics
        var random = new Random();
        return Task.FromResult(players.Select(p => new PlayerMetrics
        {
            Player = p,
            Round = round,
            Speed = random.NextDouble(),
            Accuracy = random.NextDouble(),
            ResponseOrder = random.Next(1, players.Count + 1),
            IsCorrectAnswer = true
        }));
    }

    /// <summary>
    /// Generates a list of color-word pairs for the Color Tap game.
    /// </summary>
    /// <param name="startTime">The start time of the round.</param>
    /// <param name="endTime">The end time of the round.</param>
    /// <returns>A list of ColorTapWordPairDisplay objects representing the generated pairs.</returns>
    private List<ColorTapWordPairDisplay> GenerateColorWordPairs(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var availableColors = new List<Color> { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };
        var roundDuration = (endTime - startTime).TotalMilliseconds;
        var numberOfPairs = (int)Math.Ceiling(roundDuration / _config.WordDisplayDuration.TotalMilliseconds);

        var random = new Random();
        var pairs = new List<ColorTapWordPairDisplay>();
        Color? previousWord = null;
        var wasLastPairIncorrect = false;

        for (var i = 0; i < numberOfPairs; i++)
        {
            var (randomColor, wordColor) = GenerateColorWordPair(availableColors, previousWord, wasLastPairIncorrect, random);

            wasLastPairIncorrect = randomColor != wordColor;

            var displayTime = startTime.AddMilliseconds(i * _config.WordDisplayDuration.TotalMilliseconds);
            pairs.Add(new ColorTapWordPairDisplay
            {
                Color = randomColor,
                Word = wordColor,
                DisplayTime = displayTime
            });

            previousWord = wordColor;
        }

        EnsureAtLeastOneCorrectPair(pairs, random);

        return pairs;
    }

    private (Color randomColor, Color wordColor) GenerateColorWordPair(
        List<Color> availableColors, 
        Color? previousWord, 
        bool wasLastPairIncorrect, 
        Random random)
    {
        var randomColor = availableColors[random.Next(availableColors.Count)];
        Color wordColor;

        if (wasLastPairIncorrect && random.NextDouble() <= _config.IncorrectPairProbability && previousWord.HasValue)
        {
            wordColor = previousWord.Value;
        }
        else if (random.NextDouble() <= _config.CorrectPairProbability)
        {
            wordColor = randomColor;
        }
        else
        {
            var availableWords = availableColors.Where(c => c != randomColor).ToList();
            wordColor = availableWords[random.Next(availableWords.Count)];
        }

        return (randomColor, wordColor);
    }

    private static void EnsureAtLeastOneCorrectPair(List<ColorTapWordPairDisplay> pairs, Random random)
    {
        if (pairs.Any(p => p.Color == p.Word)) return;
        
        var randomIndex = random.Next(pairs.Count);
        pairs[randomIndex].Word = pairs[randomIndex].Color;
    }
}