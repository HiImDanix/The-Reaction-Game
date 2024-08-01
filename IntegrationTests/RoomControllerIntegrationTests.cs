using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using Infrastructure;
using Xunit;
using Contracts.Input;
using Contracts.Output;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using DbContext = Infrastructure.DbContext;

namespace Integration;

public class RoomControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncDisposable
{
    private const string RoomEndpoint = "/Rooms";
    private static string VerifyRoomCodeEndpoint(string code) => $"/RoomCodes/{code}";
    private const string JoinRoomEndpoint = $"{RoomEndpoint}/join";
    
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IFixture _fixture;
    private readonly HttpClient _client;

    public RoomControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(ConfigureTestServices);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
        
        // PlayerName should be between 3 and 20 characters long
        _fixture.Customize<CreateRoomRequest>(composer =>
            composer.With(x => x.PlayerName, () => 
                _fixture.Create<string>().Substring(2, 20)));
        
        _fixture.Customize<JoinRoomRequest>(composer =>
            composer.With(x => x.PlayerName, () => 
                _fixture.Create<string>().Substring(2, 20)));
    }

    [Fact]
    public async Task CreateRoom_ReturnsCreatedRoom()
    {
        // Arrange
        var request = _fixture.Create<CreateRoomRequest>();
        // Act
        var response = await _client.PostAsJsonAsync(RoomEndpoint, request);
        // Assert
        response.EnsureSuccessStatusCode();
        var roomResponse = await response.Content.ReadFromJsonAsync<RoomOut>();
        
        roomResponse.Should().NotBeNull();
        roomResponse.Id.Should().NotBeEmpty();
        roomResponse.Code.Should().NotBeEmpty();
        roomResponse.Status.Should().Be(RoomOut.RoomStatus.Lobby);

        roomResponse.Players.Should().ContainSingle()
            .Which.Should().Match<PlayerOut>(
                p => p.Id.Length != 0 && 
                     p.Id != Guid.Empty.ToString() && 
                     p.Name == request.PlayerName);
        
        roomResponse.Host.Should().Match<PlayerOut>(h => 
            h.Id == roomResponse.Players[0].Id && 
            h.Name == request.PlayerName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("s")]
    [InlineData("      ")]
    [InlineData("SomeReallyFreakingExtremelyLongDisplayNameThatSomePersonChoseJustForFun")]
    public async Task CreateRoom_Returns400_WhenPlayerNameIsNotValid(string playerName)
    {
        // Arrange
        var request = _fixture.Build<CreateRoomRequest>()
            .With(x => x.PlayerName, playerName)
            .Create();
        // Act
        var response = await _client.PostAsJsonAsync(RoomEndpoint, request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IsRoomJoinable_Returns200_WhenValid()
    {
        // Arrange
        var createRoomRequest = _fixture.Create<CreateRoomRequest>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomOut>();
        // Act
        var response = await _client.GetAsync(VerifyRoomCodeEndpoint(roomResponse.Code));
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task IsRoomJoinable_Returns404_WhenRoomDoesNotExist()
    {
        // Arrange
        var code = "nonExistentCode";
        // Act
        var response = await _client.GetAsync(VerifyRoomCodeEndpoint(code));
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task JoinRoom_AddsNewPlayerToRoom_WhenRoomIsJoinable()
    {
        // Arrange
        var createRoomRequest = _fixture.Create<CreateRoomRequest>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomOut>();
        var joinRoomRequest = _fixture.Build<JoinRoomRequest>()
            .With(o => o.Code, roomResponse.Code)
            .Create();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint, joinRoomRequest);
        // Assert
        response.EnsureSuccessStatusCode();
        var joinedRoomResponse = await response.Content.ReadFromJsonAsync<RoomOut>();
        joinedRoomResponse.Should().NotBeNull();
        joinedRoomResponse.Players[1].Should().Match<PlayerOut>(
                p => p.Id.Length != 0 && 
                     p.Id != Guid.Empty.ToString() && 
                     p.Name == joinRoomRequest.PlayerName);
        joinedRoomResponse.Host.Should().Match<PlayerOut>(h => 
            h.Id == roomResponse.Players[0].Id && 
            h.Name == roomResponse.Players[0].Name);
    }
    
    [Fact]
    public async Task JoinRoom_Returns404_WhenRoomDoesNotExist()
    {
        // Arrange
        var joinRoomRequest = _fixture.Create<JoinRoomRequest>();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint, joinRoomRequest);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("s")]
    [InlineData("      ")]
    [InlineData("SomeReallyFreakingExtremelyLongDisplayNameThatSomePersonChoseJustForFun")]
    public async Task JoinRoom_Returns400_WhenPlayerNameIsNotValid(string playerName)
    {
        // Arrange
        var createRoomRequest = _fixture.Create<CreateRoomRequest>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomOut>();
        var joinRoomRequest = _fixture.Build<JoinRoomRequest>()
            .With(o => o.Code, roomResponse.Code)
            .With(o => o.PlayerName, playerName)
            .Create();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint, joinRoomRequest);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static void ConfigureTestServices(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

}
