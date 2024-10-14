using Domain.MiniGames;

namespace Application.Gaming;

public interface IMiniGameEngineFactory
{
    IMiniGameEngine Create(MiniGame miniGame);
}