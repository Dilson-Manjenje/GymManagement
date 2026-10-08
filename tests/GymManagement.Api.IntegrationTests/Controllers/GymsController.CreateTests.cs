using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;

namespace GymManagement.Api.IntegrationTests.Controllers.Gym;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateGymTests
{
    private readonly HttpClient _client;

    public CreateGymTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateGym_WhenRequestIsValid_CreateGym()
    {
        // Arrange
        var request = new GymRequest("Gym-Two", "Prenda");
        
        // Act
        var httpResponse = await _client.PostAsJsonAsync("Gyms", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Gyms/{resourceId.Id}");
    }

}