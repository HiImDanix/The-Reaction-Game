using Microsoft.AspNetCore.Mvc;
using Application;
using Contracts.Input;

namespace ReaktlyC.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ResultControllerBase
{
    
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRoom(CreateRoomRequest request)
    {
        var room = await _roomService.CreateRoomAsync(request.PlayerName);
        return FromResult(room);
    }
    
    [HttpGet("{code}")]
    public async Task<IActionResult> GetRoom(string code)
    {
        var room = await _roomService.GetRoomByCodeAsync(code);
        return FromResult(room);
    }

    [HttpPost("{code}/player")]
    public async Task<IActionResult> JoinRoom(string code, [FromForm] string playerName)
    {
        var room = await _roomService.JoinRoomAsync(code, playerName);
        return FromResult(room);
    }
    
    [HttpGet("player/{sessionToken}")]
    public async Task<IActionResult> GetRoomByPlayerSession(string sessionToken)
    {
        var room = await _roomService.GetRoomByYourPlayerSessionAsync(sessionToken);
        return FromResult(room);
    }
}