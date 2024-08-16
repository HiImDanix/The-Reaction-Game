using Application;
using Contracts.Output;
using Contracts.Output.Hub;
using Microsoft.AspNetCore.SignalR;

namespace ReaktlyC.Hubs;

public class LobbyHub: BaseHub, ILobbyHub
{
    
    private readonly IHubContext<LobbyHub> _context;
    
    private enum MessageType
    {
        PlayerJoined
    }

    public LobbyHub(ILogger<LobbyHub> logger, IAuthService authService, IHubContext<LobbyHub> context) : base(logger, authService, context)
    {
        _context = context;
    }
    
    public async Task NotifyPlayerJoined(string roomId, PlayerJoinedMessage msg)
    {
        await SendToRoom(roomId, MessageType.PlayerJoined.ToString(), msg);
    }
    
    
}