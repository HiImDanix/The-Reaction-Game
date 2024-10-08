using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Domain.Constants;
using Shared.Enums;

namespace Domain.MiniGames;

public class ColorTapGame : MiniGame
{
    public ColorTapGame(int rounds, TimeSpan roundDuration) : base(
        ColorTapConstants.Name,
        MiniGameType.ColorTap,
        ColorTapConstants.Instructions,
        ColorTapConstants.InstructionsDuration,
        rounds,
        roundDuration
        )
    {
    }
    
    protected ColorTapGame()
    {
    }

    public override MiniGameRound CreateRound(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        return new ColorTapRound(startTime, endTime);
    }
}
