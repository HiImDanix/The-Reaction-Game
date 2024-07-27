namespace Contracts.Output;

public record PlayerPersonalOut
{
    public string Id { get; set; }
    public string SessionToken { get; set; }
    public string Name { get; set; }
}