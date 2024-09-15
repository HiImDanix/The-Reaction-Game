namespace Domain.Constants;

public static class ColorTapConstants
{
    public const string Name = "Color Tap";
    public const string Instructions = "Tap when the color matches the word!";
    public static readonly TimeSpan InstructionsDuration = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan RoundDuration = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan WordDisplayDuration = TimeSpan.FromSeconds(1.5);
}