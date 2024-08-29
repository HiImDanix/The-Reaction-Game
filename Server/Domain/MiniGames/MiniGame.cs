using System.ComponentModel.DataAnnotations;

namespace Domain.MiniGames;

public abstract class MiniGame
{
    public enum MiniGameType
    {
        ColorTap,
    }
    
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public MiniGameType Type { get; set; }
    public string Instructions { get; set; }
    public int RoundCount { get; set; }
    public int CurrentRound { get; set; }
    public TimeSpan RoundDuration { get; set; }
    public DateTime? InstructionsStartTime { get; set; }
    public TimeSpan InstructionsDuration { get; set; }
    
    public DateTime? InstructionsEndTime => InstructionsStartTime?.Add(InstructionsDuration);
    
    protected MiniGame(string name, MiniGameType type, string instructions, TimeSpan instructionsDuration, int roundCount, TimeSpan roundDuration)
    {
        Name = name;
        Type = type;
        Instructions = instructions;
        RoundCount = roundCount;
        RoundDuration = roundDuration;
        InstructionsDuration = instructionsDuration;
        CurrentRound = 0;
    }
    
    protected MiniGame()
    {
    }
}