using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    public enum GameStatus
    {
        NotStarted,
        PrepareToStart,
        InProgress,
        Finished
    }
    
    [Key]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();
    public MiniGame? CurrentMiniGame { get; set; } = null;
    public ICollection<PlayerScore> PlayerScores { get; set; } = new List<PlayerScore>();
    public GameStatus Status { get; set; } = GameStatus.NotStarted;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public TimeSpan PreparationTime { get; set; } = TimeSpan.FromSeconds(4);
}