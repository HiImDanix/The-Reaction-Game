using Domain.MiniGames;

namespace Application.Gaming;

public class ScoringSystem: IScoringSystem
{
    private const int BaseScore = 100;
    private const double SpeedWeight = 0.5;
    private const double AccuracyWeight = 0.5;
    
    private readonly int[] _orderBonusScores = { 50, 30, 20, 15, 10 };
    
    public IEnumerable<ScoreboardLine> CalculateRoundScores(IEnumerable<PlayerMetrics> playerMetrics)
    {
        var normalizedMetrics = NormalizeMetrics(playerMetrics);

        return normalizedMetrics.Select(metrics => new ScoreboardLine
        {
            Player = metrics.Player,
            Score = CalculateScore(metrics)
        });
    }
    
    private IEnumerable<PlayerMetrics> NormalizeMetrics(IEnumerable<PlayerMetrics> playerMetrics)
    {
        var metrics = playerMetrics.ToList();
        var maxSpeed = metrics.Max(m => m.Speed);
        var maxAccuracy = metrics.Max(m => m.Accuracy);
        
        return metrics.Select(m => new PlayerMetrics
        {
            Player = m.Player,
            Round = m.Round,
            Speed = m.Speed / maxSpeed,
            Accuracy = m.Accuracy / maxAccuracy,
            ResponseOrder = m.ResponseOrder
        });
    }

    private int CalculateScore(PlayerMetrics metrics)
    {
        var orderScore = metrics.ResponseOrder <= _orderBonusScores.Length
            ? _orderBonusScores[metrics.ResponseOrder - 1]
            : 0;
        
        if (!metrics.IsCorrect)
        {
            return 0;
        }

        var score = orderScore + BaseScore * (
            metrics.Speed * SpeedWeight + 
            metrics.Accuracy * AccuracyWeight
            );
        
        return (int) Math.Round(score);
    }
}