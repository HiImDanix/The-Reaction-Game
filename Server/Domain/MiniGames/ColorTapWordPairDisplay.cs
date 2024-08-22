using System.Drawing;

namespace Domain.MiniGames;

public class ColorTapWordPairDisplay
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Color Color { get; set; }
    public Color Word { get; set; }
    public DateTime DisplayTime { get; set; }
}