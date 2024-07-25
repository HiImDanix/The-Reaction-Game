namespace Models;

public class Room
{
    // Status - Lobby, STARTING, IN_PROGRESS, FINISHED (an ENUM)
    public enum Status
    {
        Lobby,
        Starting,
        InProgress,
        Finished
    }
    
    public string Id { get; set; }
    public string Code { get; set; }
    public Status RoomStatus { get; set; }
    private List<Player> Players { get; set; }
    
}