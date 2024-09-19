using Domain;
using Domain.MiniGames;
using Shared.Enums;

namespace Application.Gaming;

public static class MiniGameFactory
{
    public static MiniGame CreateMiniGame(MiniGameType type, int rounds, TimeSpan roundDuration)
    {
        return type switch
        {
            MiniGameType.ColorTap => new ColorTapGame(rounds, roundDuration),
            _ => throw new InvalidOperationException($"Mini game type {type} is not supported")
        };
    }
}