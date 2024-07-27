using Contracts.Output;
using Domain;

namespace Application;

public interface IRoomService
{
    Task<RoomOut> CreateRoomAsync(string hostName);
    Task<RoomOut> JoinRoomAsync(string code, string playerName);
    Task<RoomOut> GetRoomByCodeAsync(string code);
    Task<RoomOut> GetRoomByYourPlayerSessionAsync(string sessionToken);
}