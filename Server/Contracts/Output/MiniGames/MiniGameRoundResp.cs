using Domain.MiniGames;

namespace Contracts.Output.MiniGames;

public record MiniGameRoundResp
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public List<ScoreboardLineResp> Scoreboard { get; set; }
}