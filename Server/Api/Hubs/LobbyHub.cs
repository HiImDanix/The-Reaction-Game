using Application;
using Contracts.Output;
using Contracts.Output.Hub;
using Domain;
using Domain.MiniGames;
using Microsoft.AspNetCore.SignalR;

namespace ReaktlyC.Hubs;

public class LobbyHub: BaseHub, ILobbyHub
{
    
    private readonly IHubContext<LobbyHub> _context;
    
    private enum MessageType
    {
        PlayerJoined,
        CurrentGameUpdated,
        CurrentMiniGameUpdated,
    }

    public LobbyHub(ILogger<LobbyHub> logger, IAuthService authService, IHubContext<LobbyHub> context) : base(logger, authService, context)
    {
        _context = context;
    }
    
    public async Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage msg)
    {
        await SendToRoom(roomId, MessageType.PlayerJoined.ToString(), msg);
    }
    
    public Task NotifyCurrentGameUpdated(string roomId, GameResp currentGame)
    {
        return SendToRoom(roomId, MessageType.CurrentGameUpdated.ToString(), currentGame);
    }

    public Task NotifyCurrentMiniGameUpdated(string roomId, MiniGameResp message)
    {
        return SendToRoom(roomId, MessageType.CurrentMiniGameUpdated.ToString(), message);
    }
}