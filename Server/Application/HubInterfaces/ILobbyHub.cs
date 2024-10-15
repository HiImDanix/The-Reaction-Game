using Contracts.Output.Hub;

namespace Application.HubInterfaces;

public interface ILobbyHub
{
    Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage dto);
}