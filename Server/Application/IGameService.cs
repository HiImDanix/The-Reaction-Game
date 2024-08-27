using FluentResults;

namespace Application;

public interface IGameService
{
    Task<Result> StartGame(string roomId);
}