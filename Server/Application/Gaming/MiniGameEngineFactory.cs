using Domain.MiniGames;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Gaming;

public class MiniGameEngineFactory: IMiniGameEngineFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public MiniGameEngineFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IMiniGameEngine Create(MiniGame miniGame)
    {
        return miniGame switch
        {
            ColorTapGame => _serviceProvider.GetRequiredService<IColorTapEngine>(),
            _ => throw new NotSupportedException($"Mini game of type {miniGame.GetType().Name} is not supported")
        };
    }
}