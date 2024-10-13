namespace Contracts.Output.MiniGames;

public class ScoreboardLineResp
{
    public string Id { get; set; }
    public PlayerResp Player { get; set; }
    public int Score { get; set; }
}