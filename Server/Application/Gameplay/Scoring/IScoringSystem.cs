using Domain.MiniGames;

namespace Application.Gameplay.Scoring;

public interface IScoringSystem
{
    public IEnumerable<ScoreboardLine> CalculateRoundScores(IEnumerable<PlayerMetrics> playerMetrics);
}