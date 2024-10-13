using System.ComponentModel.DataAnnotations;

namespace Domain.MiniGames;

public class ScoreboardLine
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Player Player { get; set; }
    public int Score { get; set; }
}