using Contracts.Output;
using FluentResults;

namespace Application;

public interface IRoomService
{
    Task<Result<RoomCreatedPersonalResp>> CreateRoomAsync(string hostName);
    Task<Result<RoomJoinedPersonalResp>> JoinRoomAsync(string code, string playerName);
    Task<Result<bool>> IsRoomJoinable(string code);
    Task<Result<RoomResp>> GetRoomById(string roomId);
}