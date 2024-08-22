namespace Domain.MiniGames;

public class ColorTapRound: IMiniGameRound
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<ColorTapWordPairDisplay> ColorWordPairs { get; set; } = new();
    public Dictionary<Player, DateTime> PlayerTapTimes { get; set; } = new();
    public DateTime StartTime { get; set; }
}