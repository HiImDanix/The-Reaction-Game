using System.Drawing;

namespace Contracts.Output.MiniGames;

public class ColorTapWordPairDisplayResp
{
    public Color Color { get; set; }
    public string Word { get; set; }
    public DateTime DisplayTime { get; set; }
}