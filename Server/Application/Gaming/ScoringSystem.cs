using Domain.MiniGames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Gaming;

public class ScoringSystem: IScoringSystem
{
    private readonly ScoringConfig _config;
    private readonly ILogger<ScoringSystem> _logger;
    
    public ScoringSystem(IOptions<ScoringConfig> config, ILogger<ScoringSystem> logger)
    {
        _config = config.Value;
        _logger = logger;
    }
    
    /// <summary>
    /// Calculates the scores for all players in a round.
    /// </summary>
    /// <param name="playerMetrics">A collection of player metrics that includes speed, accuracy, and response order.</param>
    /// <returns>A collection of scoreboard lines, each containing a player and their calculated score.</returns>
    public IEnumerable<ScoreboardLine> CalculateRoundScores(IEnumerable<PlayerMetrics> playerMetrics)
    {
        if (playerMetrics == null)
        {
            throw new ArgumentNullException(nameof(playerMetrics));
        }
        
        var playerMetricsList = playerMetrics.ToList();
        
        if (playerMetricsList.Count == 0)
        {
            return new List<ScoreboardLine>();
        }
        
        var normalizedMetrics = NormalizeMetrics(playerMetricsList);

        return normalizedMetrics.Select(metrics => new ScoreboardLine
        {
            Player = metrics.Player,
            Score = CalculateScore(metrics)
        }).ToList();
    }
    
    /// <summary>
    /// Normalizes player metrics by scaling speed and accuracy based on the maximum values across all players.
    /// If maxSpeed or maxAccuracy is zero, normalized values are set to zero for that metric to prevent division by zero.
    /// </summary>
    /// <param name="playerMetrics">A collection of player metrics to normalize.</param>
    /// <returns>A collection of normalized player metrics.</returns>
    private static IReadOnlyList<PlayerMetrics> NormalizeMetrics(IReadOnlyList<PlayerMetrics> playerMetrics)
    {
        var metrics = playerMetrics.ToList();
        var maxSpeed = metrics.Max(m => m.Speed);
        var maxAccuracy = metrics.Max(m => m.Accuracy);

        return metrics.Select(m => new PlayerMetrics
        {
            Player = m.Player,
            Round = m.Round,
            Speed = maxSpeed > 0 ? m.Speed / maxSpeed : 0,
            Accuracy = maxAccuracy > 0 ? m.Accuracy / maxAccuracy : 0,
            ResponseOrder = m.ResponseOrder
        }).ToList();
    }
    
    /// <summary>
    /// Calculates the final score for a player based on their normalized metrics.
    /// </summary>
    /// <param name="metrics">The normalized metrics for a player.</param>
    /// <returns>The final calculated score for the player.</returns>
    private int CalculateScore(PlayerMetrics metrics)
    {
        var responseOrderBonusScore = _config.OrderBonusScores.ElementAtOrDefault(metrics.ResponseOrder - 1);
        
        if (!metrics.IsCorrectAnswer)
        {
            return 0;
        }
        
        // debug
        _logger.LogInformation("Speed: {Speed}, Accuracy: {Accuracy}, ResponseOrder: {ResponseOrder}, Score: {Score}",
            metrics.Speed, metrics.Accuracy, metrics.ResponseOrder, responseOrderBonusScore);
        _logger.LogInformation("SpeedWeight: {SpeedWeight}, AccuracyWeight: {AccuracyWeight}, BaseScore: {BaseScore}",
            _config.SpeedWeight, _config.AccuracyWeight, _config.BaseScore);
        
        var weightedScore = _config.BaseScore * (
            metrics.Speed * _config.SpeedWeight + 
            metrics.Accuracy * _config.AccuracyWeight
            );
        
        return (int) Math.Round(weightedScore + responseOrderBonusScore);
    }
}