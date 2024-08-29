using Domain;
using Domain.MiniGames;

namespace Application.Gaming;

public static class MiniGameFactory
{
    public static MiniGame CreateMiniGame(MiniGame.MiniGameType type, int rounds, TimeSpan roundDuration)
    {
        return type switch
        {
            MiniGame.MiniGameType.ColorTap => new ColorTapGame(rounds, roundDuration),
            _ => throw new InvalidOperationException($"Mini game type {type} is not supported")
        };
    }
}