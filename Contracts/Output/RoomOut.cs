namespace Contracts.Output;

public record RoomOut
{
    public string Id { get; set; }
    public string Code { get; set; }
    public RoomStatus Status { get; set; }
    public List<PlayerOut> Players { get; set; } = new();
    public PlayerOut Host { get; set; }
    public List<GameOut> Games { get; set; } = new();
    public GameOut CurrentGame { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlayerPersonalOut You { get; set; }

    public enum RoomStatus
    {
        Lobby,
        Starting,
        InProgress,
        Finished
    }
}