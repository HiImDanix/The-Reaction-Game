using Contracts.Output;
using Domain;
using FluentResults;

namespace Application;

public interface IRoomService
{
    Task<Result<RoomOut>> CreateRoomAsync(string hostName);
    Task<Result<RoomOut>> JoinRoomAsync(string code, string playerName);
    Task<Result<RoomOut>> GetRoomByCodeAsync(string code);
    Task<Result<RoomOut>> GetRoomByYourPlayerSessionAsync(string sessionToken);
}