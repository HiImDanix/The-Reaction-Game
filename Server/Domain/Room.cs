using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Constants;

namespace Domain;

public class Room
{
    private static readonly Random Random = new Random();
    
    public enum RoomStatus
    {
        Lobby,
        Starting,
        InProgress,
        Finished
    }
    
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    [MaxLength(RoomConstants.MaxCodeLength)]
    public string Code { get; set; }
    public RoomStatus Status { get; set; }
    [Range(1, RoomConstants.MaxPlayers)]
    public List<Player> Players { get; private set; }  = new();
    [Required]
    public Player Host { get; set; }
    public List<Game> PastGames { get; set; } = new();
    public Game? CurrentGame { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    

    public Room(Player host)
    {
        Code = GenerateRoomCode();
        Status = RoomStatus.Lobby;
        Host = host;
        Players.Add(host);
    }
    
    // EF constructor
    public Room()
    {}

    // TODO: Extract to service
    private static string GenerateRoomCode()
    {
       var generatedCode = new StringBuilder(RoomConstants.MaxCodeLength);
       for (var i = 0; i < RoomConstants.MaxCodeLength; i++)
       {
           generatedCode.Append(RoomConstants.CodeChars[Random.Next(RoomConstants.CodeChars.Length)]);
       }
       
       return generatedCode.ToString();
    }

}