using Domain;
using Domain.MiniGames;

namespace Application.Gaming;

public interface IMiniGameEngine
{
    public Task PlayCurrentRound(Room room, MiniGame miniGame);
    public Task<IEnumerable<PlayerMetrics>> CalculatePlayerMetrics(Room room, MiniGameRound miniGameRound);
}