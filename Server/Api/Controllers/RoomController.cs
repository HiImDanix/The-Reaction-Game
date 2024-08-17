using Microsoft.AspNetCore.Mvc;
using Application;
using Contracts.Input;
using Microsoft.AspNetCore.Authorization;
using ReaktlyC.Attributes;
using ReaktlyC.Extensions;

namespace ReaktlyC.Controllers;

[Route("rooms")]
public class RoomController : ResultControllerBase
{
    
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateRoom(CreateRoomReq req)
    {
        var room = await _roomService.CreateRoomAsync(req.PlayerName);
        return ResponseFromResult(room);
    }
    
    [HttpGet("by-code/{code}/joinable")]
    public async Task<IActionResult> IsRoomJoinable(string code)
    {
        var room = await _roomService.IsRoomJoinable(code);
        return room.IsSuccess ? Ok() : ResponseFromErrors(room.Errors);
    }
    
    [HttpPost("by-code/{code}/players")]
    public async Task<IActionResult> JoinRoom(string code, JoinRoomReq req)
    {
        var room = await _roomService.JoinRoomAsync(code, req.PlayerName);
        return ResponseFromResult(room);
    }
    
    
    [HttpGet("me")]
    [Authorize(Policy = "PlayerAuth")]
    public async Task<IActionResult> GetRoomByPlayerSession()
    {
        var roomId = HttpContext.GetRoomId();
        var room = await _roomService.GetRoomById(roomId);
        return ResponseFromResult(room);
    }
    
    [HttpPost("me/start-game")]
    [Authorize(Policy = "PlayerAuth")]
    public async Task<IActionResult> StartGame()
    {
        var roomId = HttpContext.GetRoomId();
        await _roomService.StartGame(roomId);
        return Ok();
    }
}