namespace Contracts.Output;

public record GameResp
{
    public string Id { get; set; }
    public GameStatus Status { get; set; }
    public DateTime PreparationStartTime { get; set; }
    public DateTime PreparationEndTime { get; set; }
    
    
    public enum GameStatus
    {
        Lobby,
        PrepareToStart,
        InProgress,
        FinalScoreboard
    }
}