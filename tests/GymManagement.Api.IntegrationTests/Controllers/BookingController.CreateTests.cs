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

namespace GymManagement.Api.IntegrationTests.Controllers.Bookings;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateBookingTests
{
    private readonly HttpClient _client;

    public CreateBookingTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateBooking_WhenRequestIsValid_CreateBooking()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Luanda");
        var room = await CreateRoom("Room-One", gym.Id);
        var memberTrainer = await CreateMember("Trainer-One", "password123", gym.Id);
        var trainer = await CreateTrainer("Trainer-One", "trainer1@gmail.com", memberTrainer.Id);
        var session = await CreateSession("Morning Yoga", room.Id, trainer.Id);
        var participant = await CreateMember("Participante-One", "password123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, participant.Id);
        await AddRoomToSubscription(subscription.Id, room.Id);

        var request = new CreateBookingRequest(
            MemberId: participant.Id,
            SessionId: session.Id
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Bookings", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Bookings/{resourceId.Id}");
    }

    private async Task<GymResponse> CreateGym(string name, string address)
    {
        // Act
        var httpResponse = await _client.PostAsJsonAsync("Gyms", new GymRequest(name, address));

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var gymResponse = await _client.GetFromJsonAsync<GymResponse>($"Gyms/{resourceId.Id}");
        gymResponse.Should().NotBeNull();

        return gymResponse;
    }

    private async Task<RoomResponse> CreateRoom(string name, Guid gymId)
    {
        // Arrange
        var request = new RoomRequest(Name: name, Capacity: 2, GymId: gymId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Rooms", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"Rooms/{resourceId.Id}");
        roomResponse.Should().NotBeNull();

        return roomResponse;
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
    
    private async Task<TrainerResponse> CreateTrainer(string name, string email, Guid memberId)
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: name,
            Phone: "953000111",
            Email: email,
            Specialization: "Yoga",
            MemberId: memberId
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Trainers", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var trainerResponse = await _client.GetFromJsonAsync<TrainerResponse>($"Trainers/{resourceId.Id}");
        trainerResponse.Should().NotBeNull();

        return trainerResponse;
    }

    private async Task<SessionResponse> CreateSession(string title, Guid roomId, Guid trainerId)
    {
        var request = new CreateSessionRequest(
            RoomId: roomId,
            TrainerId: trainerId,
            Title: title,
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        var httpResponse = await _client.PostAsJsonAsync("Sessions", request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"Sessions/{resourceId.Id}");
        sessionResponse.Should().NotBeNull();

        return sessionResponse;
    }

    private async Task<SubscriptionResponse> CreateSubscription(SubstriptionType type, Guid memberId)
    {
        // Arrange
        var request = new CreateSubscriptionRequest(SubscriptionType: type, memberId);

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Subscriptions", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var subscriptionResponse = await _client.GetFromJsonAsync<SubscriptionResponse>($"Subscriptions/{resourceId.Id}");
        subscriptionResponse.Should().NotBeNull();

        return subscriptionResponse;
    }

    private async Task<bool> AddRoomToSubscription(Guid subscriptionId, Guid roomId)
    {
        // Arrange
        var request = new RoomSubscriptionRequest(SubscriptionId: subscriptionId, RoomId: roomId);

        // Act
        var httpResponse = await _client.PutAsJsonAsync($"Subscriptions/{subscriptionId}/Rooms", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
  
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        
        return resourceId is not null;
    }
}