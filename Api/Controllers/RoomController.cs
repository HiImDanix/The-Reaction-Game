using Microsoft.AspNetCore.Mvc;
using Application;
using Contracts.Input;

namespace ReaktlyC.Controllers;

[Route("rooms")]
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
        return ResponseFromResult(room);
    }
    
    [HttpGet("roomCode")]
    public async Task<IActionResult> VerifyRoomIsJoinable(string code)
    {
        var room = await _roomService.IsRoomJoinable(code);
        
        return room.IsSuccess ? Ok() : ResponseFromErrors(room.Errors);
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinRoom(JoinRoomRequest req)
    {
        var room = await _roomService.JoinRoomAsync(req.Code, req.PlayerName);
        return ResponseFromResult(room);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRoomByPlayerSession()
    {
        var sessionToken = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(sessionToken))
        {
            return Unauthorized("Authorization header is missing");
        }
        
        // TODO: Authorize the player. Create a filter for this.
        
        var room = await _roomService.GetRoomByPlayerSessionAsync(sessionToken);
        return ResponseFromResult(room);
    }
}