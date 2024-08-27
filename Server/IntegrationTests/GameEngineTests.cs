using Application;
using Contracts.Output.Hub;
using Domain;
using Domain.MiniGames;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Integration;

public class GameEngineTests
{
    // [Fact]
    // public async Task GameEngine_Works()
    // {
    //     var player = new Player(name: "Daniel");
    //     var room = new Room(player);
    //     var miniGame = new ColorTapGame(2, TimeSpan.FromSeconds(5));
    //     room.Game.MiniGames.Add(miniGame);
    //     // Mock db context
    //     var context = new Mock<Repository>();
    //
    //     var gameEngine = new GameEngine(room, context.Object, new Mock<ILobbyHub>().Object);
    // }

    [Fact]
    public async Task GameEngine_Works()
    {
        var player = new Player(name: "Daniel");
        var room = new Room(player);
        var miniGame = new ColorTapGame(2, TimeSpan.FromSeconds(5));
        room.Game.MiniGames.Add(miniGame);

        // in memory db conext
        var options = new DbContextOptionsBuilder<Repository>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        var context = new Repository(options);

        // Mock ILobbyHub
        var lobbyHub = new Mock<ILobbyHub>();
        lobbyHub.SetupAllProperties();
        lobbyHub.Setup(m => m.NotifyPlayerJoined(It.IsAny<string>(), It.IsAny<PlayerJoinedMessage>()))
            .Callback(() => System.Diagnostics.Debug.WriteLine("ILobbyHub.NotifyPlayerJoined called"))
            .Returns(Task.CompletedTask);
        lobbyHub.Setup(m => m.NotifyMiniGameStartedShowInstructions(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Callback(
                () => System.Diagnostics.Debug.WriteLine("ILobbyHub.NotifyMiniGameStartedShowInstructions called"))
            .Returns(Task.CompletedTask);
        lobbyHub.Setup(m => m.NotifyGameStatusChanged(It.IsAny<string>(), It.IsAny<Game.GameStatus>()))
            .Callback(() => System.Diagnostics.Debug.WriteLine("ILobbyHub.NotifyGameStatusChanged called"))
            .Returns(Task.CompletedTask);
        
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug) // Set the minimum log level
                .AddConsole(); // Add console logging
        });
        // Create logger instances for your classes
        var gameEngineLogger = loggerFactory.CreateLogger<GameEngine>();

        var gameEngine = new GameEngine(room, context, lobbyHub.Object, gameEngineLogger);
        
        

        await gameEngine.StartAsync(CancellationToken.None);
    }
}