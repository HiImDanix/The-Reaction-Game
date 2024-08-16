using Contracts.Output;
using Contracts.Output.Hub;

namespace Application;

public interface ILobbyHub
{
    Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage dto);
}