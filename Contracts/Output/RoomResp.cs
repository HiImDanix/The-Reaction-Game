namespace Contracts.Output;

public record RoomResp
{
    public string Id { get; set; }
    public string Code { get; set; }
    public RoomStatus Status { get; set; }
    public List<PlayerResp> Players { get; set; } = new();
    public PlayerResp Host { get; set; }
    public List<GameResp> Games { get; set; } = new();
    public GameResp CurrentGame { get; set; }
    public DateTime CreatedAt { get; set; }

    public enum RoomStatus
    {
        Lobby,
        Starting,
        InProgress,
        Finished
    }
}