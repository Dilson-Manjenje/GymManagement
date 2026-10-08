using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Trainers;

namespace GymManagement.Api.IntegrationTests.Controllers.Trainers;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateTrainerTests
{
    private readonly HttpClient _client;

    public CreateTrainerTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateTrainer_WhenRequestIsValid_CreateTrainer()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var request = new CreateTrainerRequest("Raimundo",
                                               "929001002",
                                               "raimundo.pt@victorygym.com",
                                               "body builder",
                                               member.Id);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Trainers", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Trainers/{resourceId.Id}");
    }

    private async Task<MemberResponse> CreateMember(string userName, string password, Guid gymId)
    {
        // Arrange
        var request = new CreateMemberRequest(UserName: userName, Password: password, GymId: gymId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Members", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var memberResponse = await _client.GetFromJsonAsync<MemberResponse>($"Members/{resourceId.Id}");
        memberResponse.Should().NotBeNull();

        return memberResponse;

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