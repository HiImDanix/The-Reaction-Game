using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Domain.Constants;

namespace Domain.MiniGames;

public class ColorTapGame : MiniGame
{
    public static readonly TimeSpan TimeBetweenColorChange = TimeSpan.FromSeconds(1.5);
    public List<ColorTapRound> Rounds { get; set; } = new();
    
    public ColorTapGame(int rounds, TimeSpan roundDuration) : base(
        ColorTapConstants.Name,
        MiniGameType.ColorTap,
        ColorTapConstants.Instructions,
        rounds,
        roundDuration
        )
    {
    }
    
    protected ColorTapGame()
    {
    }
    
}
