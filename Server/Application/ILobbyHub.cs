using Contracts.Output;
using Contracts.Output.Hub;
using Domain;

namespace Application;

// TODO: Rename
public interface ILobbyHub
{
    Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage dto);
    Task NotifyCurrentGameUpdated(string roomId, GameResp currentGame);
}