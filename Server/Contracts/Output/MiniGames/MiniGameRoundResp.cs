namespace Contracts.Output.MiniGames;

public record MiniGameRoundResp
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}