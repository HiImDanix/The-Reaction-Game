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
    private static string VerifyRoomCodeEndpoint(string code) => $"rooms/by-code/{code}/joinable";
    private static string JoinRoomEndpoint(string code) => $"{RoomEndpoint}/by-code/{code}/players";
    
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IFixture _fixture;
    private readonly HttpClient _client;

    public RoomControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(ConfigureTestServices);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
        
        // PlayerName should be between 3 and 20 characters long
        _fixture.Customize<CreateRoomReq>(composer =>
            composer.With(x => x.PlayerName, () => 
                _fixture.Create<string>().Substring(2, 20)));
        
        _fixture.Customize<JoinRoomReq>(composer =>
            composer.With(x => x.PlayerName, () => 
                _fixture.Create<string>().Substring(2, 20)));
    }

    [Fact]
    public async Task CreateRoom_ReturnsCreatedRoom()
    {
        // Arrange
        var request = _fixture.Create<CreateRoomReq>();
        // Act
        var response = await _client.PostAsJsonAsync(RoomEndpoint, request);
        // Assert: Successful response
        response.EnsureSuccessStatusCode();
        var roomCreatedResponse = await response.Content.ReadFromJsonAsync<RoomCreatedPersonalResp>();
        // Assert: session token and created player details exist
        roomCreatedResponse.SessionToken.Should().NotBeEmpty();
        roomCreatedResponse.You.Should().Match<PlayerResp>(
            p => p.Id.Length != 0 && 
                 p.Id != Guid.Empty.ToString() && 
                 p.Name == request.PlayerName);
        // Assert: room details
        var room = roomCreatedResponse.Room;
        room.Id.Should().NotBeEmpty();
        room.Code.Should().NotBeEmpty();
        room.Status.Should().Be(RoomResp.RoomStatus.Lobby);
        room.Players.Should().ContainSingle()
            .Which.Should().Match<PlayerResp>(
                p => p.Id.Length != 0 && 
                     p.Id != Guid.Empty.ToString() && 
                     p.Name == request.PlayerName);
        room.Host.Should().Match<PlayerResp>(h => 
            h.Id == room.Players[0].Id && 
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
        var request = _fixture.Build<CreateRoomReq>()
            .With(x => x.PlayerName, playerName)
            .Create();
        // Act
        var response = await _client.PostAsJsonAsync(RoomEndpoint, request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IsRoomCodeValid_Returns200_WhenValid()
    {
        // Arrange
        var createRoomRequest = _fixture.Create<CreateRoomReq>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomCreatedPersonalResp>();
        // Act
        var response = await _client.GetAsync(VerifyRoomCodeEndpoint(roomResponse.Room.Code));
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
        var createRoomRequest = _fixture.Create<CreateRoomReq>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomCreatedPersonalResp>();
        var joinRoomRequest = _fixture.Create<JoinRoomReq>();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint(roomResponse.Room.Code), joinRoomRequest);
        // Assert: Successful response
        response.EnsureSuccessStatusCode();
        var roomJoinedResp = await response.Content.ReadFromJsonAsync<RoomJoinedPersonalResp>();
        // Assert: session token and created player details exist
        roomJoinedResp.SessionToken.Should().NotBeEmpty();
        roomJoinedResp.You.Should().Match<PlayerResp>(
            p => p.Id.Length != 0 && 
                 p.Id != Guid.Empty.ToString() && 
                 p.Name == joinRoomRequest.PlayerName);
        // Assert: room details
        var room = roomJoinedResp.Room;
        room.Should().NotBeNull();
        room.Players[1].Should().Match<PlayerResp>(
                p => p.Id.Length != 0 && 
                     p.Id != Guid.Empty.ToString() && 
                     p.Name == joinRoomRequest.PlayerName);
        room.Host.Should().Match<PlayerResp>(h => 
            h.Id == room.Players[0].Id && 
            h.Name == room.Players[0].Name);
    }
    
    [Fact]
    public async Task JoinRoom_Returns404_WhenRoomDoesNotExist()
    {
        // Arrange
        var joinRoomRequest = _fixture.Create<JoinRoomReq>();
        var roomCode = _fixture.Create<string>();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint(roomCode), joinRoomRequest);
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
        var createRoomRequest = _fixture.Create<CreateRoomReq>();
        var createRoomResponse = await _client.PostAsJsonAsync(RoomEndpoint, createRoomRequest);
        var roomResponse = await createRoomResponse.Content.ReadFromJsonAsync<RoomResp>();
        var joinRoomRequest = _fixture.Build<JoinRoomReq>()
            .With(o => o.PlayerName, playerName)
            .Create();
        // Act
        var response = await _client.PostAsJsonAsync(JoinRoomEndpoint(roomResponse.Code), joinRoomRequest);
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
