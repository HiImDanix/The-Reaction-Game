namespace Contracts.Output;

public record MiniGameResp
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Instructions { get; set; }
    public DateTime? InstructionsStartTime { get; set; }
    public DateTime? InstructionsEndTime { get; set; }
}