using Application;
using Application.HubInterfaces;
using Contracts.Output;
using Contracts.Output.Hub;
using Contracts.Output.MiniGames;
using Domain;
using Domain.MiniGames;
using Microsoft.AspNetCore.SignalR;

namespace ReaktlyC.Hubs;

public class GameplayHub: BaseHub, IGameplayHub
{
    
    private readonly IHubContext<LobbyHub> _context;
    
    private enum MessageType
    {
        CurrentGameUpdated,
        CurrentMiniGameUpdated,
        CurrentRoundUpdated
    }

    public GameplayHub(ILogger<GameplayHub> logger, IAuthService authService, IHubContext<LobbyHub> context) : base(logger, authService, context)
    {
        _context = context;
    }
    
    public Task NotifyCurrentGameUpdated(string roomId, GameResp currentGame)
    {
        return SendToRoom(roomId, MessageType.CurrentGameUpdated.ToString(), currentGame);
    }

    public Task NotifyCurrentMiniGameUpdated(string roomId, MiniGameResp message)
    {
        return SendToRoom(roomId, MessageType.CurrentMiniGameUpdated.ToString(), message);
    }

    public Task NotifyCurrentRoundUpdated(string roomId, MiniGameRoundResp roundDto)
    {
        return SendToRoom(roomId, MessageType.CurrentRoundUpdated.ToString(), roundDto);
    }
}