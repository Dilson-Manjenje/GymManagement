using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Subscriptions;
using TestCommon.TestConstants;

namespace GymManagement.Api.IntegrationTests.Controllers.Subscriptions;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class SubscriptionApiTests
{
    private readonly HttpClient _client;

    public SubscriptionApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateSubscription_WhenRequestIsValid_CreateSubscription()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var request = new CreateSubscriptionRequest(SubscriptionType: SubstriptionType.Plus, member.Id);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Subscriptions", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Subscriptions/{resourceId.Id}");
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

    // [Theory]
    // [MemberData(nameof(ListSubscriptionTypes))]
    // public async Task CreateSubscription_WhenValidSubscription_ShouldCreateSubscription(SubstriptionType subscriptionType)
    // {
    //     // Arrange
    //     var createSubscriptionRequest = new CreateSubscriptionRequest(
    //         SubscriptionType: subscriptionType,
    //         MemberId: Constants.Members.AdminId);

    //     // Act
    //     var response = await _client.PostAsJsonAsync("Subscriptions", createSubscriptionRequest);

    //     // Assert
    //     response.StatusCode.Should().Be(HttpStatusCode.Created);
    //     response.Headers.Location.Should().NotBeNull();

    //     var subscriptionResponse = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
    //     subscriptionResponse.Should().NotBeNull();
    //     subscriptionResponse!.SubscriptionType.Should().Be(subscriptionType);

    //     response.Headers.Location!.PathAndQuery.Should().Be($"/Subscriptions/{subscriptionResponse.Id}");
    // }

    // public static TheoryData<SubstriptionType> ListSubscriptionTypes()
    // {
    //     var subscriptionTypes = Enum.GetValues<SubstriptionType>().ToList();

    //     var theoryData = new TheoryData<SubstriptionType>();

    //     foreach (var subscriptionType in subscriptionTypes)
    //     {
    //         theoryData.Add(subscriptionType);
    //     }

    //     return theoryData;
    // }
}
