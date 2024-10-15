using Contracts.Output;
using Contracts.Output.MiniGames;

namespace Application.HubInterfaces;

public interface IGameplayHub
{
    Task NotifyCurrentGameUpdated(string roomId, GameResp currentGame);
    Task NotifyCurrentMiniGameUpdated(string roomId, MiniGameResp miniGame);
    Task NotifyCurrentRoundUpdated(string roomId, MiniGameRoundResp roundDto);
}