using Domain.MiniGames;

namespace Application.Gameplay;

public interface IMiniGameEngineFactory
{
    IMiniGameEngine Create(MiniGame miniGame);
}