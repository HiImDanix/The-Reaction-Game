using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ReaktlyC.Attributes;
using ReaktlyC.Extensions;

namespace ReaktlyC.Hubs;

[Authorize(Policy = "PlayerAuth")]
public class BaseHub: Hub
{
    private readonly ILogger<BaseHub> _logger;
    private readonly IAuthService _authService;
    private readonly IHubContext<BaseHub> _context;

    public BaseHub(ILogger<BaseHub> logger, IAuthService authService, IHubContext<BaseHub> context)
    {
        _logger = logger;
        _authService = authService;
        _context = context;
    }
    
    private enum GroupType
    {
        Player,
        Room
    }

    public override Task OnConnectedAsync()
    {
        var playerId = Context.GetHttpContext()?.GetPlayerId() ?? throw new HubException("Player ID not found in claims.");
        var roomId = Context.GetHttpContext()?.GetRoomId() ?? throw new HubException("Room ID not found in claims.");
        _logger.LogInformation("Player {PlayerId} connected to the hub", playerId);
        _context.Groups.AddToGroupAsync(Context.ConnectionId, $"{GroupType.Player}:{playerId}");
        _context.Groups.AddToGroupAsync(Context.ConnectionId, $"{GroupType.Room}:{roomId}");
        return base.OnConnectedAsync();
    }
    
    protected async Task SendToPlayer(string playerId, string method, object data)
    {
        await _context.Clients.Group($"{GroupType.Player}:{playerId}").SendAsync(method, data);
    }
    
    protected async Task SendToRoom(string roomId, string method, object data)
    {
        await _context.Clients.Group($"{GroupType.Room}:{roomId}").SendAsync(method, data);
    }
    
    protected async Task SendToRoomExceptCaller(string roomId, string method, object data)
    {
        
        await Task.FromException<bool>(new NotImplementedException());
        
    }
}