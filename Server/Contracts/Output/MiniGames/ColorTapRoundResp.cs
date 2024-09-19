namespace Contracts.Output.MiniGames;

public record ColorTapRoundResp: MiniGameRoundResp
{
    public List<ColorTapWordPairDisplayResp> ColorWordPairs { get; init; }
}