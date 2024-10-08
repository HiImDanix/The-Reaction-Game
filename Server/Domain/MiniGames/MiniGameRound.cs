namespace Domain.MiniGames;

public abstract class MiniGameRound
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
}