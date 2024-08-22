namespace Domain;



public abstract class MiniGame
{
    public enum MiniGameType
    {
        ColorTap,
    }
    
    public string Id { get; } = Guid.NewGuid().ToString();
    public string Name { get; }
    public MiniGameType Type { get; }
    public string Instructions { get; }
    public int RoundCount { get; }
    public int CurrentRound { get; set; }
    public TimeSpan RoundDuration { get; }
    public DateTime InstructionStartTime { get; set; }
    
    protected MiniGame(string name, MiniGameType type, string instructions, int roundCount, TimeSpan roundDuration)
    {
        Name = name;
        Type = type;
        Instructions = instructions;
        RoundCount = roundCount;
        RoundDuration = roundDuration;
    }
}