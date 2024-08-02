using Microsoft.AspNetCore.Mvc;
using Application;
using Contracts.Input;
using ReaktlyC.Middlewares;

namespace ReaktlyC.Controllers;

public class RoomController : ResultControllerBase
{
    
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(CreateRoomReq req)
    {
        
        var room = await _roomService.CreateRoomAsync(req.PlayerName);
        return ResponseFromResult(room);
    }
    
    [HttpGet("RoomCodes/{code}")]
    public async Task<IActionResult> IsRoomJoinable(string code)
    {
        var room = await _roomService.IsRoomJoinable(code);
        
        return room.IsSuccess ? Ok() : ResponseFromErrors(room.Errors);
    }
    
    [HttpPost("rooms/join")]
    public async Task<IActionResult> JoinRoom(JoinRoomReq req)
    {
        var room = await _roomService.JoinRoomAsync(req.Code, req.PlayerName);
        return ResponseFromResult(room);
    }
    
    [RequirePlayerAuth]
    [HttpGet("rooms/me")]
    public async Task<IActionResult> GetRoomByPlayerSession()
    {
        var roomId = HttpContext.GetRoomId();
        var room = await _roomService.GetRoomById(roomId);
        return ResponseFromResult(room);
    }
}