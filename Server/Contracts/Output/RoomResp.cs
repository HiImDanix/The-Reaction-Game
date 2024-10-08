namespace Contracts.Output;

public record RoomResp
{
    public string Id { get; set; }
    public string Code { get; set; }
    public List<PlayerResp> Players { get; set; } = new();
    public PlayerResp Host { get; set; }
    public List<GameResp> PastGames { get; set; } = new();
    public GameResp CurrentGame { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}