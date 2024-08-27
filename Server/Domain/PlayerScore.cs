using System.ComponentModel.DataAnnotations;

namespace Domain;

public class PlayerScore
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public int Score { get; set; }
    
    public Game Game { get; set; }
    public Player Player { get; set; }
    
    public PlayerScore(string gameId, string playerId, int score)
    {
        GameId = gameId;
        PlayerId = playerId;
        Score = score;
    }
    
    public PlayerScore()
    {
    }
}