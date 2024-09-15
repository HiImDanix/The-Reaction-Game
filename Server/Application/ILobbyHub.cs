using Contracts.Output;
using Contracts.Output.Hub;
using Domain;
using Domain.MiniGames;

namespace Application;

// TODO: Rename
public interface ILobbyHub
{
    Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage dto);
    Task NotifyCurrentGameUpdated(string roomId, GameResp currentGame);
    Task NotifyColorTapRoundStarted(string roomId, ColorTapRound round); // TODO: Change to DTO
}