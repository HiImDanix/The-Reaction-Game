namespace Domain.MiniGames;

public class ColorTapRound: MiniGameRound
{
    public List<ColorTapWordPairDisplay> ColorWordPairs { get; set; } = new();
    // public Dictionary<Player, DateTime> PlayerTapTimes { get; set; } = new();
    
    public ColorTapRound(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}