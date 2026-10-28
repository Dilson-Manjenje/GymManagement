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
using GymManagement.Contracts.Trainers;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.IntegrationTests.Controllers.Sessions;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class SessionApiTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private GymResponse _gym = null!;
    private RoomResponse _room = null!;
    private MemberResponse _trainerMember = null!;
    private TrainerResponse _trainer = null!;

    public SessionApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    public async Task InitializeAsync()
    {
        _gym = await CreateGym("Gym-One", "Adress-One");
        _room = await CreateRoom("Room-One", _gym.Id);
        _trainerMember = await CreateMember("trainer1", "Pass123", _gym.Id);
        _trainer = await CreateTrainer("Raimundo", "trainer1@gmal.com", "950111222", _trainerMember.Id);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

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
        var request = new CreateRoomRequest(Name: name, Capacity: 2, GymId: gymId);

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

    [Fact]
    public async Task CreateSession_WhenRequestIsValid_ReturnsCreated()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var request = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: start.AddHours(2)
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Sessions.Create, request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"{ApiEndPoints.Sessions.Base}/{resourceId.Id}");
    }

    [Fact]
    public async Task GetSession_WithInvalidId_ReturnsNotFound()
    {

        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Sessions.Base}/{Guid.NewGuid()}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateSession_WithValidRequest_ReturnsOK()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: start.AddHours(2)
        );
        var session = await CreateSession(createRequest);

        // Arrange
        var updateRequest = new UpdateSessionRequest(
            Id: session.Id,
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 2",
            StartDate: DateTime.Now.AddDays(1),
            EndDate: DateTime.Now.AddDays(1).AddHours(1)
        );

        // Act
        var httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}", updateRequest);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var response = await _client.GetFromJsonAsync<SessionResponse>($"{ApiEndPoints.Sessions.Base}/{resourceId?.Id}");
        response.Should().BeOfType<SessionResponse>();
        response.Title.Should().BeEquivalentTo(updateRequest.Title);
        response.StartDate.Should().Be(updateRequest.StartDate);
        response.EndDate.Should().Be(updateRequest.EndDate);
    }

    [Fact]
    public async Task DeleteSession_WhenIdIsValid_ReturnsNoContent()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: start.AddHours(2)
        );
        var session = await CreateSession(createRequest);

        // Act
        var httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    private async Task<SessionResponse> CreateSession(CreateSessionRequest request)
    {
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Sessions.Create, request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"{ApiEndPoints.Sessions.Base}/{resourceId.Id}");
        sessionResponse.Should().NotBeNull();

        return sessionResponse;
    }


    [Fact]
    public async Task CancelSession_WithValidRequest_ReturnsOK()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: start.AddHours(2)
        );
        var session = await CreateSession(createRequest);

        // Arrange
        // Act
        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}/Cancel", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"{ApiEndPoints.Sessions.Base}/{resourceId?.Id}");
        sessionResponse.Should().BeOfType<SessionResponse>();
        sessionResponse.Status.Should().Be(SessionStatusType.Canceled);
    }

    [Fact]
    public async Task FinalizeSession_WithValidRequest_ReturnsOK()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: start.AddHours(2)
        );
        var session = await CreateSession(createRequest);

        // Act
        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}/Finalize", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"{ApiEndPoints.Sessions.Base}/{resourceId?.Id}");
        sessionResponse.Should().BeOfType<SessionResponse>();
        sessionResponse.Status.Should().Be(SessionStatusType.Finalized);
    }

    [Fact]
    public async Task ListSessions_ReturnOKWithAllSessions()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var end = start.AddMinutes(65);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: end
        );
        await CreateSession(createRequest);
        
        createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 2",
            StartDate: end.AddMinutes(5),
            EndDate: end.AddMinutes(65)
        );
        await CreateSession(createRequest);
        
        // Act
        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Sessions.List}");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListSessionsResponse>();

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListSessionsResponse>();
        response.Data.Should().NotBeNull(); 
        response.Data.Count().Should().Be(2);
    }

    [Fact]
    public async Task ListUpCommingSessions_ReturnOKWithActiveSessions()
    {
        // Arrange
        var start = DateTime.Now.AddMinutes(5);
        var end = start.AddMinutes(65);
        var createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 1",
            StartDate: start,
            EndDate: end
        );
        var session1 = await CreateSession(createRequest);

        createRequest = new CreateSessionRequest(
            RoomId: _room.Id,
            TrainerId: _trainer.Id,
            Title: "Session 2",
            StartDate: end.AddMinutes(5),
            EndDate: end.AddMinutes(65)
        );
        await CreateSession(createRequest);

        var httpResponse = await _client.PutAsync($"{ApiEndPoints.Sessions.Base}/{session1.Id}/Cancel", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        httpResponse = await _client.GetAsync($"{ApiEndPoints.Sessions.Base}/UpCommingSessions/{_gym.Id}");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListSessionsResponse>();

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListSessionsResponse>();
        response.Data?.Count().Should().Be(1);
    }

    [Fact]
    public async Task ListSessionActiveBookings_ReturnOkWithBookings()
    {               
        var gym = await CreateGym("Gym-Two", "Luanda");
        var room = await CreateRoom("Room-Two", gym.Id);
        var memberTrainer = await CreateMember("trainer.two", "password123", gym.Id);
        var trainer = await CreateTrainer("trainer-two", "trainer2@gmail.com", "953000222", memberTrainer.Id);
        
        var createRequest = new CreateSessionRequest(
            RoomId: room.Id,
            TrainerId: trainer.Id,
            Title: "Session 3"
        );
        var session = await CreateSession(createRequest);
        var participant = await CreateMember("Participante-One", "password123", gym.Id);
        var subscription = await CreateSubscription(SubstriptionType.Plus, participant.Id);
        await AddRoomToSubscription(subscription.Id, room.Id);
        var booking = await CreateBooking(session.Id, participant.Id);

        await _client.PutAsJsonAsync($"{ApiEndPoints.Bookings.Base}/{booking.Id}/Cancel", booking.Id);
        await CreateBooking(session.Id, participant.Id);

        // Act        
        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}/ActiveBookings");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListBookingResponse>();

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListBookingResponse>();
        response.Data?.Count().Should().Be(1);
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

    [Fact]
    public async Task ListSessionBookings_ReturnOkWithBookings()
    {               
        var gym = await CreateGym("Gym-Two", "Luanda");
        var room = await CreateRoom("Room-Two", gym.Id);
        var memberTrainer = await CreateMember("trainer.two", "password123", gym.Id);
        var trainer = await CreateTrainer("trainer-two", "trainer2@gmail.com", "953000011", memberTrainer.Id);
        
        var createRequest = new CreateSessionRequest(
            RoomId: room.Id,
            TrainerId: trainer.Id,
            Title: "Session 3"
        );
        var session = await CreateSession(createRequest);
        var participant1 = await CreateMember("Participante-One", "password123", gym.Id);
        var subscription1 = await CreateSubscription(SubstriptionType.Plus, participant1.Id);
        await AddRoomToSubscription(subscription1.Id, room.Id);
        await CreateBooking(session.Id, participant1.Id);

        var participant2 = await CreateMember("Participante-Two", "password123", gym.Id);
        var subscription2 = await CreateSubscription(SubstriptionType.Plus, participant2.Id);
        await AddRoomToSubscription(subscription2.Id, room.Id);
        await CreateBooking(session.Id, participant2.Id);

        // Act        
        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Sessions.Base}/{session.Id}/AllBookings");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListBookingResponse>();

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListBookingResponse>();
        response.Data?.Count().Should().Be(2);
    }
}
