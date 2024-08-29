using Domain;
using Domain.MiniGames;

namespace Application.Gaming;

public interface IColorTapEngine: IMiniGameEngine
{
    public Task PlayRound(Room room, MiniGame miniGame);
}