using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Constants;
using Domain.MiniGames;

namespace Domain;

public class Game
{
    // TODO: change enum names, also dto and frontend
    public enum GameStatus
    {
        Lobby,
        PrepareToStart,
        InProgress,
        FinalScoreboard
    }
    
    [Key]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();
    public MiniGame? CurrentMiniGame { get; set; }
    // public ICollection<PlayerScore> Scoreboard { get; set; } = new List<PlayerScore>();
    public GameStatus Status { get; set; } = GameStatus.Lobby;
    public DateTimeOffset? StartClickedAt { get; set; }
    [NotMapped]
    public DateTimeOffset? PreparationStartTime => StartClickedAt;
    public TimeSpan PreparationDuration { get; set; } = GameConstants.PreparationTime;
    [NotMapped]
    public DateTimeOffset? PreparationEndTime => StartClickedAt?.Add(PreparationDuration);
}