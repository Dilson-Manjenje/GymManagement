using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Rooms;

namespace GymManagement.Api.IntegrationTests.Controllers.Rooms;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateRoomTests
{
    private readonly HttpClient _client;

    public CreateRoomTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateRoom_WhenRequestIsValid_CreateGym()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");
        var request = new RoomRequest(Name: "Room 1", 2, gym.Id);

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
        // Arrange
        var request = new GymRequest(name, address);
        
        // Act
        var httpResponse = await _client.PostAsJsonAsync("Gyms", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();        
        
        var gymResponse = await _client.GetFromJsonAsync<GymResponse>($"Gyms/{resourceId.Id}");
        gymResponse.Should().NotBeNull();

        return gymResponse;
    }

}