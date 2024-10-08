namespace Contracts.Output;

public record GameResp
{
    public string Id { get; set; }
    public GameStatus Status { get; set; }
    public DateTimeOffset PreparationStartTime { get; set; }
    public DateTimeOffset PreparationEndTime { get; set; }
    public MiniGameResp CurrentMiniGame { get; set; }
    
    
    public enum GameStatus
    {
        Lobby,
        PrepareToStart,
        InProgress,
        FinalScoreboard
    }
}