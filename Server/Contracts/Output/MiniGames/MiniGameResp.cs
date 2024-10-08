using Contracts.Output.MiniGames;

namespace Contracts.Output;

public class MiniGameResp
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Instructions { get; set; }
    public DateTimeOffset? InstructionsStartTime { get; set; }
    public DateTimeOffset? InstructionsEndTime { get; set; }
    public int TotalRoundsNo { get; set; }
    public int CurrentRoundNo { get; set; }
    public object CurrentRound { get; set; } // Object due to polymorphism (different mini game round types)
}