using Domain.MiniGames;

namespace Application.Gaming;

public interface IScoringSystem
{
    public IEnumerable<ScoreboardLine> CalculateRoundScores(IEnumerable<PlayerMetrics> playerMetrics);
}