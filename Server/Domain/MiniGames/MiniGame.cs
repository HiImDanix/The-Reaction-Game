using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;



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
    public int CurrentRound { get; set; } = 1;
    public TimeSpan RoundDuration { get; set; }
    public DateTime InstructionStartTime { get; set; }
    public TimeSpan InstructionsDuration { get; set; } = TimeSpan.FromSeconds(5); // TODO: Make this configurable
    
    protected MiniGame(string name, MiniGameType type, string instructions, int roundCount, TimeSpan roundDuration)
    {
        Name = name;
        Type = type;
        Instructions = instructions;
        RoundCount = roundCount;
        RoundDuration = roundDuration;
    }
    
    protected MiniGame()
    {
    }
}