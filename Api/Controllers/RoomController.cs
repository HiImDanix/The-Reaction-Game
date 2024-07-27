using Microsoft.AspNetCore.Mvc;
using Application;

namespace ReaktlyC.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromForm] string playerName)
    {
        var room = await _roomService.CreateRoomAsync(playerName);
        return Ok(room);
    }
    
    [HttpGet("{code}")]
    public async Task<IActionResult> GetRoom(string code)
    {
        var room = await _roomService.GetRoomByCodeAsync(code);
        return Ok(room);
    }

    [HttpPost("{code}/player")]
    public async Task<IActionResult> JoinRoom(string code, [FromForm] string playerName)
    {
        var room = await _roomService.JoinRoomAsync(code, playerName);
        return Ok(room);
    }
    
    // Get room by player session token
    [HttpGet("player/{sessionToken}")]
    public async Task<IActionResult> GetRoomByPlayerSession(string sessionToken)
    {
        var room = await _roomService.GetRoomByYourPlayerSessionAsync(sessionToken);
        return Ok(room);
    }
}