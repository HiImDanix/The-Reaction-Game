namespace Application.Gaming;

public class ColorTapConfig
{
    public double IncorrectPairProbability { get; set; } = 0.50;
    public double CorrectPairProbability { get; set; } = 0.20;
    public TimeSpan WordDisplayDuration = TimeSpan.FromSeconds(2.5);
}