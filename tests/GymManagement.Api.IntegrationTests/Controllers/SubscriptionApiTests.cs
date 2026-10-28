using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Rooms;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        httpResponse.Headers.Location.PathAndQuery.Should().Be($"{ApiEndPoints.Subscriptions.Base}/{resourceId.Id}");
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

    [Fact]
    public async Task GetSubscription_WithInvalidId_ReturnsNotFound()
    {

        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Subscriptions.Base}/{Guid.NewGuid()}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task DisableSubscription_WithValidId_ReturnOK()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, member.Id);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Disable", null);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var response = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{resourceId?.Id}");
        response.Should().BeOfType<SubscriptionResponse>();
        response.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateSubscription_WithValidRequest_ReturnOK()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Basic, member.Id);

        var request = new UpdateSubscriptionRequest(subscription.Id, SubstriptionType.Plus);

        var httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}", request);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}");
        response.Should().BeOfType<SubscriptionResponse>();
        response.IsActive.Should().BeTrue();
        response.SubscriptionType.Should().Be(SubstriptionType.Plus);
    }
    
    [Fact]
    public async Task DeleteSubscription_AfterDisabled_ReturnsNoContent()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Basic, member.Id);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Disable", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        httpResponse = await _client.GetAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    private async Task<SubscriptionResponse> CreateSubscription(SubstriptionType type, Guid memberId)
    {
        var request = new CreateSubscriptionRequest(type, memberId);
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Subscriptions.Create, request);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var response = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{resourceId.Id}");

        response.Should().NotBeNull();
        response.Should().BeOfType<SubscriptionResponse>();

        return response;
    }

    [Fact]
    public async Task AddRoomToSubscription_WithValidRoomId_ReturnOK()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, member.Id);
        var room1 = await CreateRoom("Room-One", gym.Id);
        var room2 = await CreateRoom("Room-Two", gym.Id);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room1.Id}", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);                     

        httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room2.Id}", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}");
        response.Should().BeOfType<SubscriptionResponse>();
        response.Rooms.Should().Contain(room1.Name);
        response.Rooms.Should().Contain(room2.Name);
    } 
    private async Task<RoomResponse> CreateRoom(string name, Guid gymId)
    {
        // Arrange
        var request = new CreateRoomRequest(Name: name, Capacity: 2, GymId: gymId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}");
        roomResponse.Should().NotBeNull();

        return roomResponse;
    }

    [Fact]
    public async Task RemoveRoomFromSubscription_WhenRoomsBelongsToSubscription_ReturnNoContent()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, member.Id);
        var room1 = await CreateRoom("Room-One", gym.Id);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room1.Id}", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room1.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}");
        response.Should().BeOfType<SubscriptionResponse>();
        response.Rooms.Should().NotContain(room1.Name);
    }

    [Fact]
    public async Task ListSubscriptionRooms_ReturnOKWithRooms()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("dilson", "abc123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, member.Id);
        var room1 = await CreateRoom("Room-One", gym.Id);
        var room2 = await CreateRoom("Room-Two", gym.Id);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room1.Id}", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms/{room2.Id}", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetFromJsonAsync<ListRoomsResponse>($"{ApiEndPoints.Subscriptions.Base}/{subscription.Id}/Rooms");
        response.Should().BeOfType<ListRoomsResponse>();
        response.Data.Should().NotBeNull();
        response.Data.Count().Should().Be(2);
    } 
    
    [Fact]
    public async Task ListSubscription_ReturnOKWithAllSubscriptions()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Luanda");
        var member1 = await CreateMember("member1", "abc123", gym.Id);
        await CreateSubscription(SubstriptionType.Basic, member1.Id);

        var gym2 = await CreateGym("Gym-Two", "Luanda");
        var member2 = await CreateMember("member2", "abc123", gym2.Id);
        await CreateSubscription(SubstriptionType.Plus, member2.Id);


        // Act
        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Subscriptions.List}");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListSubscriptionsResponse>();

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListSubscriptionsResponse>();
        response.Data.Should().NotBeNull();
        response.Data.Count().Should().Be(2);
    }
    
    [Theory]
    [MemberData(nameof(ListSubscriptionTypes))]
    public async Task CreateSubscription_WhenSubscriptionTypeIsValid_returnsCreated(SubstriptionType subscriptionType)
    {
        var gym = await CreateGym("Gym-One", "Luanda");
        var member = await CreateMember("member-one", "Pass123", gym.Id);

        // Arrange
        var createSubscriptionRequest = new CreateSubscriptionRequest(
            SubscriptionType: subscriptionType,
            MemberId: member.Id);

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Subscriptions.Create, createSubscriptionRequest);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();
                
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty(); 
        
        httpResponse = await _client.GetAsync($"{ApiEndPoints.Subscriptions.Base}/{resourceId.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await httpResponse.Content.ReadFromJsonAsync<SubscriptionResponse>();

        response.Should().NotBeNull();    
        response.SubscriptionType.Should().Be(subscriptionType);
    }

    public static TheoryData<SubstriptionType> ListSubscriptionTypes()
    {
        var subscriptionTypes = Enum.GetValues<SubstriptionType>().ToList();

        var theoryData = new TheoryData<SubstriptionType>();

        foreach (var subscriptionType in subscriptionTypes)
        {
            theoryData.Add(subscriptionType);
        }

        return theoryData;
    }
}
