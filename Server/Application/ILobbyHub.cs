using Contracts.Output;
using Contracts.Output.Hub;
using Domain;

namespace Application;

// TODO: Rename
public interface ILobbyHub
{
    Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage dto);
    Task NotifyMiniGameStartedShowInstructions(string roomId, string miniGameName, string miniGameInstructions, TimeSpan miniGameInstructionsDuration);
    Task NotifyGameStatusChanged(string roomId, Game.GameStatus inProgress);
}