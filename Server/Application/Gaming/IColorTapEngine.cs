using Domain;
using Domain.MiniGames;

namespace Application.Gaming;

public interface IColorTapEngine: IMiniGameEngine
{
    public Task PlayCurrentRound(Room room, MiniGame miniGame);
}