namespace Domain.MiniGames;

public class ColorTapRound: MiniGameRound
{
    public List<ColorTapWordPairDisplay> ColorWordPairs { get; set; } = new();
    // public Dictionary<Player, DateTime> PlayerTapTimes { get; set; } = new();
    
    public ColorTapRound(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}