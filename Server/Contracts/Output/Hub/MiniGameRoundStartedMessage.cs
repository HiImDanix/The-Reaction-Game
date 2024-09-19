using Shared.Enums;

namespace Contracts.Output.Hub;

public class MiniGameRoundStartedMessage
{
    public MiniGameType MiniGameType { get; set; }
    public string MiniGameName { get; set; }
    public int RoundNo { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public object RoundData { get; set; }
}