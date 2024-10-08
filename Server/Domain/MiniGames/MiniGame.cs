using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Domain.MiniGames;

public abstract class MiniGame
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public MiniGameType Type { get; set; }
    public string Instructions { get; set; }
    public int TotalRoundsNo { get; set; }
    public int CurrentRoundNo { get; set; }
    public MiniGameRound? CurrentRound { get; set; }
    public TimeSpan RoundDuration { get; set; }
    public DateTimeOffset? InstructionsStartTime { get; set; }
    public TimeSpan InstructionsDuration { get; set; }
    public DateTimeOffset? InstructionsEndTime => InstructionsStartTime?.Add(InstructionsDuration);
    public IList<MiniGameRound> Rounds { get; set; } = new List<MiniGameRound>();
    
    public abstract MiniGameRound CreateRound(DateTimeOffset startTime, DateTimeOffset endTime);
    
    protected MiniGame(string name, MiniGameType type, string instructions, TimeSpan instructionsDuration, int totalRoundsNo, TimeSpan roundDuration)
    {
        Name = name;
        Type = type;
        Instructions = instructions;
        TotalRoundsNo = totalRoundsNo;
        RoundDuration = roundDuration;
        InstructionsDuration = instructionsDuration;
        CurrentRoundNo = 0;
    }
    
    protected MiniGame()
    {
    }
}