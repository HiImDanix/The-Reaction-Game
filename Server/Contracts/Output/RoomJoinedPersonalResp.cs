namespace Contracts.Output;

public class RoomJoinedPersonalResp
{
    public RoomResp Room { get; set; }
    public PlayerResp You { get; set; }
    public string SessionToken { get; set; }
}