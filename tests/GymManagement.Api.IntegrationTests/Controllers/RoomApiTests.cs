using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Rooms;

namespace GymManagement.Api.IntegrationTests.Controllers.Rooms;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class RoomApiTests
{
    private readonly HttpClient _client;

    public RoomApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateRoom_WhenRequestIsValid_CreateGym()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");
        var request = new CreateRoomRequest(Name: "Room 1", 2, gym.Id);

        // Act
        var response = await _client.PostAsJsonAsync("Rooms", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var resourceId = await response.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        response.Headers.Location!.PathAndQuery.Should().Be($"/Rooms/{resourceId.Id}");
    }

    private async Task<GymResponse> CreateGym(string name, string address)
    {
        var request = new GymRequest(name, address);

        var httpResponse = await _client.PostAsJsonAsync("Gyms", request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var gymResponse = await _client.GetFromJsonAsync<GymResponse>($"Gyms/{resourceId.Id}");
        gymResponse.Should().NotBeNull();

        return gymResponse;
    }

    [Fact]
    public async Task UpdateRoom_WhenRequestIsValid_UpdateRoom()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");
        var httpResponse = await _client.PostAsJsonAsync("Rooms", new CreateRoomRequest(Name: "Room 1", 2, gym.Id));
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();


        // act
        var request = new UpdateRoomRequest(Id: resourceId.Id, Name: "Room-Two", Capacity: 5);
        httpResponse = await _client.PutAsJsonAsync($"Rooms/{request.Id}", request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"Rooms/{resourceId!.Id}");
        roomResponse.Should().NotBeNull();
        roomResponse.Name.Should().BeEquivalentTo(request.Name);
        roomResponse.Capacity.Should().Be(request.Capacity);
    }

    [Fact]
    public async Task DisableRoom_WhenRequestIsValid_DisableRoom()
    {
        var gym = await CreateGym("Gym-Two", "Prenda");
        var httpResponse = await _client.PostAsJsonAsync("Rooms", new CreateRoomRequest(Name: "Room 1", 2, gym.Id));
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        httpResponse = await _client.DeleteAsync($"Rooms/{resourceId.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"Rooms/{resourceId!.Id}");
        roomResponse.Should().NotBeNull();
        roomResponse.IsAvailable.Should().BeFalse();
    }
}