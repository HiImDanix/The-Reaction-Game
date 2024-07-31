using Contracts.Output;
using Domain;
using FluentResults;

namespace Application;

public interface IRoomService
{
    Task<Result<RoomOut>> CreateRoomAsync(string hostName);
    Task<Result<RoomOut>> JoinRoomAsync(string code, string playerName);
    Task<Result<RoomOut>> GetRoomByPlayerSessionAsync(string sessionToken);
    Task<Result<bool>> IsRoomJoinable(string code);
}