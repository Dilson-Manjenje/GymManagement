using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Bookings;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Rooms;
using GymManagement.Contracts.Sessions;
using GymManagement.Contracts.Subscriptions;
using GymManagement.Contracts.Trainers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.IntegrationTests.Controllers.Members;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class MembersApiTests
{
    private readonly HttpClient _client;

    public MembersApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateMember_WhenRequestIsValid_ReturnsCreated()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Members", new CreateMemberRequest(UserName: "User1", "ABC123", gym.Id));

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"{ApiEndPoints.Members.Base}/{resourceId.Id}");
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
    public async Task UpdateMember_WhenRequestIsValid_ReturnsOK()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Prenda");
        var member = await CreateMember("Member-One", "Pass123", gym.Id);

        // act
        var request = new UpdateMemberRequest("Member-Two", "Pass567");
        var httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Members.Base}/{member.Id}", request);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        var response = await _client.GetFromJsonAsync<MemberResponse>($"{ApiEndPoints.Members.Base}/{resourceId?.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        response.Should().BeOfType<MemberResponse>();
        response.UserName.Should().BeEquivalentTo(request.UserName);
    }

    [Fact]
    public async Task Delete_WhenIRequestIsValid_ReturnsNoContent()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Prenda");
        var member = await CreateMember("Member-One", "Pass123", gym.Id);

        // act
        var httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Members.Base}/{member.Id}");

        // assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        httpResponse = await _client.GetAsync($"{ApiEndPoints.Members.Base}/{member.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // string content = await httpResponse.Content.ReadAsStringAsync();
        // var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
        var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Status.Should().Be(StatusCodes.Status404NotFound);
    }


    private async Task<MemberResponse> CreateMember(string userName, string password, Guid gymId)
    {
        // Arrange
        var request = new CreateMemberRequest(UserName: userName, Password: password, GymId: gymId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Members.Create, request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var memberResponse = await _client.GetFromJsonAsync<MemberResponse>($"{ApiEndPoints.Members.Base}/{resourceId.Id}");
        memberResponse.Should().NotBeNull();

        return memberResponse;
    }

    [Fact]
    public async Task ListMembers_ReturnListOfMembers()
    {
        var gym = await CreateGym("Gym-Two", "Prenda");
        await _client.PostAsJsonAsync(ApiEndPoints.Members.Create, new CreateMemberRequest(UserName: "Member1", "Pass123", gym.Id));
        await _client.PostAsJsonAsync(ApiEndPoints.Members.Create, new CreateMemberRequest(UserName: "Member2", "Pass123", gym.Id));

        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Members.List}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetFromJsonAsync<ListMembersResponse>($"{ApiEndPoints.Members.List}");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListMembersResponse>();
        response.Data?.Count().Should().Be(3); // N+1 (Admin seeded)

    }

    [Fact]
    public async Task ListMembersSubscriptions_ReturnCorrectSubscriptions()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        var member1 = await CreateMember("Member-One", "password123", gym.Id);
        var subs1 = await CreateSubscription(SubstriptionType.Basic, member1.Id);

        await _client.PutAsJsonAsync($"{ApiEndPoints.Subscriptions.Base}/{subs1.Id}/Disable", subs1.Id);
        await CreateSubscription(SubstriptionType.Plus, member1.Id);
        
        var member2 = await CreateMember("Member-Two", "password123", gym.Id);
        await CreateSubscription(SubstriptionType.Plus, member2.Id);
        
        var response1 = await _client.GetFromJsonAsync<ListSubscriptionsResponse>($"{ApiEndPoints.Members.Base}/{member1.Id}/Subscriptions");
        var response2 = await _client.GetFromJsonAsync<ListSubscriptionsResponse>($"{ApiEndPoints.Members.Base}/{member2.Id}/Subscriptions");

        response1.Should().NotBeNull();
        response1.Should().BeOfType<ListSubscriptionsResponse>();
        response1.Data?.Count().Should().Be(2);
        
        response2.Should().NotBeNull();
        response2.Should().BeOfType<ListSubscriptionsResponse>();
        response2.Data?.Count().Should().Be(1);

    }

    private async Task<SubscriptionResponse> CreateSubscription(SubstriptionType type, Guid memberId)
    {
        // Arrange
        var request = new CreateSubscriptionRequest(SubscriptionType: type, memberId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Subscriptions.Create, request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var subscriptionResponse = await _client.GetFromJsonAsync<SubscriptionResponse>($"{ApiEndPoints.Subscriptions.Base}/{resourceId.Id}");
        subscriptionResponse.Should().NotBeNull();

        return subscriptionResponse;
    }

    [Fact]
    public async Task ListMembersBookings_ReturnCorrectBookings()
    {
        var gym = await CreateGym("Gym-One", "Luanda");
        var room = await CreateRoom("Room-One", gym.Id);
        var memberTrainer = await CreateMember("Trainer-One", "password123", gym.Id);
        var trainer = await CreateTrainer("Trainer-One", "trainer1@gmail.com", "953000111", memberTrainer.Id);
        var session = await CreateSession("Session 1", room.Id, trainer.Id);
        var participant = await CreateMember("Participante-One", "password123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, participant.Id);
        await AddRoomToSubscription(subscription.Id, room.Id);
        var booking = await CreateBooking(session.Id, participant.Id);

        await _client.PutAsJsonAsync($"{ApiEndPoints.Bookings.Base}/{booking.Id}/Cancel", booking.Id);
        await CreateBooking(session.Id, participant.Id);

        // Act        
        var response = await _client.GetFromJsonAsync<ListBookingResponse>($"{ApiEndPoints.Members.Base}/{participant.Id}/Bookings");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ListBookingResponse>();
        response.Data?.Count().Should().Be(2);

    }

    private async Task<RoomResponse> CreateRoom(string name, Guid gymId)
    {
        // Arrange
        var request = new CreateRoomRequest(Name: name, Capacity: 2, GymId: gymId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Rooms", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}");
        roomResponse.Should().NotBeNull();

        return roomResponse;
    }
    private async Task<TrainerResponse> CreateTrainer(string name, string email, string phone, Guid memberId)
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: name,
            Phone: phone,
            Email: email,
            Specialization: "Body Builder",
            MemberId: memberId
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Trainers", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var trainerResponse = await _client.GetFromJsonAsync<TrainerResponse>($"{ApiEndPoints.Trainers.Base}/{resourceId.Id}");
        trainerResponse.Should().NotBeNull();

        return trainerResponse;
    }
    private async Task<SessionResponse> CreateSession(string title, Guid roomId, Guid trainerId, DateTime? start = null, DateTime? end = null)
    {
        var request = new CreateSessionRequest(
            RoomId: roomId,
            TrainerId: trainerId,
            Title: title,
            StartDate: start ?? DateTime.Now.AddMinutes(5),
            EndDate: end ?? DateTime.Now.AddHours(2)
        );

        var httpResponse = await _client.PostAsJsonAsync("Sessions", request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"{ApiEndPoints.Sessions.Base}/{resourceId.Id}");
        sessionResponse.Should().NotBeNull();

        return sessionResponse;
    }

    private async Task<bool> AddRoomToSubscription(Guid subscriptionId, Guid roomId)
    {
        // Act
        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Subscriptions.Base}/{subscriptionId}/Rooms/{roomId}", null);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        return resourceId is not null;
    }

    private async Task<BookingResponse> CreateBooking(Guid sessionId, Guid memberId)
    {
        var request = new CreateBookingRequest(
            MemberId: memberId,
            SessionId: sessionId
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync($"{ApiEndPoints.Bookings.Base}", request);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
               
        var response = await _client.GetFromJsonAsync<BookingResponse>($"{ApiEndPoints.Bookings.Base}/{resourceId.Id}");
        response.Should().NotBeNull();

        return response;
    }
}