namespace Contracts.Input;

public record JoinRoomRequest
{
    public string Code { get; }
    public string PlayerName { get; }
}