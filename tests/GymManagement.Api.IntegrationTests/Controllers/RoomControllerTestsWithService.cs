using FluentAssertions;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Api.IntegrationTests.Services;
using GymManagement.Contracts.Rooms;


namespace GymManagement.Api.IntegrationTests.Controllers.Rooms;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class RoomControllerTestsWithService
{
    private readonly GymService _gymService;
    private readonly RoomService _roomService;

    public RoomControllerTestsWithService(GymManagementApiFactory apiFactory)
    {
        var client = apiFactory.HttpClient;
        _gymService = new GymService(client);
        _roomService = new RoomService(client);
    }

    [Fact]
    public async Task CreateRoom_WhenRequestIsValid_CreateRoom()
    {
        // Arrange
        var gym = await _gymService.CreateGym("Gym-One-On", "Luanda");
        gym.Should().NotBeNull();

        var room = await _roomService.CreateRoom(new CreateRoomRequest("Room-One", 2, gym.Id));

        // Assert
        room.Should().NotBeNull();
        room.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRoom_WhenRequestIsNotValid_ReturnError()
    {
        // Arrange
        var gym = await _gymService.CreateGym("Gym-One", "Luanda");
        gym.Should().NotBeNull();

        var room = await _roomService.CreateRoom(new CreateRoomRequest("Ab", 2, gym.Id));

        // Assert        
        room.Should().BeNull();
    }
}