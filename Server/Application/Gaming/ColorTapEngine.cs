using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Application.Gaming;
using AutoMapper;
using Contracts.Output;
using Contracts.Output.Hub;
using Domain;
using Domain.Constants;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Application;

public class ColorTapEngine : IColorTapEngine
{
    private const double IncorrectPairProbability = 0.50;
    private const double CorrectPairProbability = 0.20;

    private readonly Repository _context;
    private readonly ILobbyHub _lobbyHub;
    private readonly ILogger<ColorTapEngine> _logger;
    private readonly IMapper _mapper;

    public ColorTapEngine(Repository context, ILobbyHub lobbyHub, ILogger<ColorTapEngine> logger, IMapper mapper)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task PlayCurrentRound(Room room, MiniGame miniGame)
    {
        if (miniGame is not ColorTapGame)
        {
            throw new InvalidOperationException("Mini game is not of type ColorTapGame");
        }
        
        if (miniGame.CurrentRound is not ColorTapRound round)
        {
            throw new InvalidOperationException("Current round is not of type ColorTapRound");
        }
        
        _logger.LogInformation("Color Tap game round {RoundNo} started (duration: {RoundDuration})",
            miniGame.CurrentRoundNo, miniGame.RoundDuration);
        
        // Generate data for the round, update the round and notify the clients
        round.ColorWordPairs = GenerateColorWordPairs(round.StartTime, round.EndTime);
        _context.ColorTapRounds.Update(round);
        await _context.SaveChangesAsync();
        await _lobbyHub.NotifyCurrentMiniGameUpdated(room.Id, _mapper.Map<MiniGameResp>(miniGame));
        
        _logger.LogInformation("Waiting for the round to finish");
        await Task.Delay(ColorTapConstants.RoundDuration);
        _logger.LogInformation("Color Tap round finished");
    }

    public Task<IEnumerable<PlayerMetrics>> CaculateRoundMetrics(Room room, MiniGameRound miniGameRound)
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
    private static List<ColorTapWordPairDisplay> GenerateColorWordPairs(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var availableColors = new List<Color> { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange };
        var roundDuration = (endTime - startTime).TotalMilliseconds;
        var numberOfPairs = (int)Math.Ceiling(roundDuration / ColorTapConstants.WordDisplayDuration.TotalMilliseconds);

        var random = new Random();
        var pairs = new List<ColorTapWordPairDisplay>();
        Color? previousWord = null;
        var wasLastPairIncorrect = false;

        for (var i = 0; i < numberOfPairs; i++)
        {
            var (randomColor, wordColor) = GenerateColorWordPair(availableColors, previousWord, wasLastPairIncorrect, random);

            wasLastPairIncorrect = randomColor != wordColor;

            var displayTime = startTime.AddMilliseconds(i * ColorTapConstants.WordDisplayDuration.TotalMilliseconds);
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

    private static (Color randomColor, Color wordColor) GenerateColorWordPair(
        List<Color> availableColors, 
        Color? previousWord, 
        bool wasLastPairIncorrect, 
        Random random)
    {
        var randomColor = availableColors[random.Next(availableColors.Count)];
        Color wordColor;

        if (wasLastPairIncorrect && random.NextDouble() <= IncorrectPairProbability && previousWord.HasValue)
        {
            wordColor = previousWord.Value;
        }
        else if (random.NextDouble() <= CorrectPairProbability)
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