using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    public enum GameStatus
    {
        NotStarted,
        Starting,
        InProgress,
        Finished
    }
    
    [Key]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public List<MiniGame> MiniGames { get; set; }
    public MiniGame CurrentMiniGame { get; set; }
    public Dictionary<Player, int> PlayerScores { get; set; }
    public GameStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan PreparationTime { get; set; } = TimeSpan.FromSeconds(4);
}