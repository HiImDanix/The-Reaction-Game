using Application;
using Contracts.Output;
using Contracts.Output.Hub;
using Domain;
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

    public Task NotifyMiniGameStartedShowInstructions(string roomId, string miniGameName, string miniGameInstructions,
        TimeSpan miniGameInstructionsDuration)
    {
        Console.WriteLine("Mini game started: show instructions");
        return Task.CompletedTask;
    }

    public Task NotifyGameStatusChanged(string roomId, Game.GameStatus inProgress)
    {
        Console.WriteLine("Game status changed");
        return Task.CompletedTask;
    }
}