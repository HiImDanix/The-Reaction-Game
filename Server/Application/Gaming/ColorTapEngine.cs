using System.Drawing;
using Application.Gaming;
using AutoMapper;
using Contracts.Output;
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
    private readonly IMapper _mapper;

    public ColorTapEngine( Repository context, ILobbyHub lobbyHub, ILogger<ColorTapEngine> logger, IMapper mapper)
    {
        _context = context;
        _lobbyHub = lobbyHub;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task PlayCurrentRound(Room room, MiniGame miniGame)
    {
        if (miniGame.CurrentRound is not ColorTapRound round)
        {
            throw new InvalidOperationException("Current round is not of type ColorTapRound");
        }
        
        _logger.LogInformation("Color Tap game round {RoundNumber} started", miniGame.CurrentRoundNo);
        _logger.LogInformation("Round duration: {RoundDuration}", miniGame.RoundDuration);
        
        // Generate color-word pairs for the round
        round.ColorWordPairs = GenerateColorWordPairs(round.StartTime, round.EndTime);
        
        // Notify the players about the round
        _logger.LogInformation("Notifying players about the round");
        _logger.LogInformation("Round details: {@Round}", round);
        await _lobbyHub.NotifyColorTapRoundStarted(room.Id, _mapper.Map<ColorTapRound>(round));
        
        _logger.LogInformation("Waiting for the round to finish");
        await Task.Delay(ColorTapConstants.RoundDuration);
        _logger.LogInformation("Color Tap round finished");
    }
    
    private static List<ColorTapWordPairDisplay> GenerateColorWordPairs(DateTime startTime, DateTime endTime)
    {
        var displayDuration = ColorTapConstants.WordDisplayDuration;
        
        var availableColors = new List<Color> { Color.Red, Color.Blue, Color.Green,
            Color.Yellow, Color.Purple, Color.Orange };
        var roundDuration = (endTime - startTime).TotalMilliseconds;
        var numberOfPairs = (int)Math.Ceiling(roundDuration / ColorTapConstants.WordDisplayDuration.TotalMilliseconds);
        
        
        var random = new Random();
        var pairs = new List<ColorTapWordPairDisplay>();
        var correctPairs = 0;

        for (var i = 0; i < numberOfPairs; i++)
        {
            var randomColor = availableColors[random.Next(availableColors.Count)];
            Color wordColor;

            if (random.NextDouble() <= 0.20)
            {
                wordColor = randomColor;
                correctPairs++;
            } 
            else
            {
                var availableWords = availableColors.Where(c => c != randomColor).ToList();
                wordColor = availableWords[random.Next(availableWords.Count)];
            }
            
            var displayTime = startTime.AddMilliseconds(i * displayDuration.TotalMilliseconds);
            
            pairs.Add(new ColorTapWordPairDisplay
            {
                Color = randomColor,
                Word = wordColor,
                DisplayTime = displayTime
            });

        }
        
        if (correctPairs == 0)
        {
            var randomIndex = random.Next(pairs.Count);
            pairs[randomIndex].Word = pairs[randomIndex].Color;
        }
        
        return pairs;
    }
    
    
}