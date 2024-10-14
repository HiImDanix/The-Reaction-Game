namespace Application.Gameplay.Scoring;

public class ScoringConfig
{
    public int BaseScore { get; set; } = 100;
    public double SpeedWeight { get; set; } = 0.5;
    public double AccuracyWeight { get; set; } = 0.5;
    public int[] OrderBonusScores { get; set; } = { 50, 30, 20, 15, 10 };
}