namespace Domain.MiniGames;

public abstract class MiniGameRound
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}