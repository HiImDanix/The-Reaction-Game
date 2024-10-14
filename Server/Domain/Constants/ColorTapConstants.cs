namespace Domain.Constants;

public static class ColorTapConstants
{
    public const string Name = "Color Tap";
    public const string Instructions = "Tap when the color matches the word!";
    public static readonly TimeSpan InstructionsDuration = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan DefaultRoundDuration = TimeSpan.FromSeconds(3);
    public static readonly int DefaultRoundsCount = 3;
}