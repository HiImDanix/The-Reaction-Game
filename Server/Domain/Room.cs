using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Domain.Constants;

namespace Domain;

public class Room
{
    private static readonly Random Random = new Random();
    
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    [MaxLength(RoomConstants.MaxCodeLength)]
    public string Code { get; set; }
    [Range(1, RoomConstants.MaxPlayers)]
    public List<Player> Players { get; private set; }  = new();
    [Required]
    public string HostId { get; set; }
    public List<Game> PastGames { get; set; } = new();
    [ForeignKey(nameof(CurrentGameId))]
    public Game CurrentGame { get; set; }
    public string CurrentGameId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    

    public Room(Player host)
    {
        Code = GenerateRoomCode();
        HostId = host.Id;
        Players.Add(host);
        CurrentGame = new Game();
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