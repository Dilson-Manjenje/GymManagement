using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;

namespace GymManagement.Api.IntegrationTests.Controllers.Members;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateMembersTests
{
    private readonly HttpClient _client;

    public CreateMembersTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateMember_WhenRequestIsValid_CreateGym()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");
        var request = new CreateMemberRequest(UserName: "User1", "ABC123", gym.Id);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Members", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Members/{resourceId.Id}");
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