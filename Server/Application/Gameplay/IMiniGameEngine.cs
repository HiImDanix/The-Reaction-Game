using Domain;
using Domain.MiniGames;

namespace Application.Gameplay;

public interface IMiniGameEngine
{
    public Task PlayCurrentRoundAsync(Room room, MiniGame miniGame);
    public Task<IEnumerable<PlayerMetrics>> CalculateRoundMetrics(Room room, MiniGameRound miniGameRound);
}