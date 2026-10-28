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

namespace GymManagement.Api.IntegrationTests.Controllers.Gym;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class GymApiTests
{
    private readonly HttpClient _client;

    public GymApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateGym_WhenRequestIsValid_ReturnsCreated()
    {
        // Arrange
        var request = new GymRequest("Gym-Two", "Prenda");

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Gyms.Create, request);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();

        // Assert
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location?.PathAndQuery.Should().Be($"{ApiEndPoints.Gyms.Base}/{resourceId.Id}");
    }


    [Fact]
    public async Task UpdateGym_WhenRequestIsValid_ReturnsOK()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Prenda");

        // act
        var request = new GymRequest(Name: "Gym-Two", Address: "Morro Bento");
        var httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Gyms.Base}/{gym.Id}", request);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        var gymResponse = await _client.GetFromJsonAsync<GymResponse>($"{ApiEndPoints.Gyms.Base}/{resourceId?.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        gymResponse.Should().NotBeNull();
        gymResponse.Should().BeOfType<GymResponse>();
        gymResponse.Name.Should().BeEquivalentTo(request.Name);
        gymResponse.Address.Should().Be(request.Address);
    }

    private async Task<GymResponse> CreateGym(string name, string address)
    {
        var request = new GymRequest(name, address);

        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Gyms.Create, request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var gymResponse = await _client.GetFromJsonAsync<GymResponse>($"{ApiEndPoints.Gyms.Base}/{resourceId.Id}");
        gymResponse.Should().NotBeNull();

        return gymResponse;
    }

    [Fact]
    public async Task Delete_WhenRequestIsValid_ReturnNoContent()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");

        // act
        var httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Gyms.Base}/{gym.Id}");

        // assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        httpResponse = await _client.GetAsync($"{ApiEndPoints.Gyms.Base}/{gym.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        //string content = await httpResponse.Content.ReadAsStringAsync();
        //var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
        var problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetails!.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task ListGym_ReturnCorrectyNumberOfGyms()
    {
        await CreateGym("Gym-One", "Prenda");
        await CreateGym("Gym-Two", "Morro Bento");

        var response = await _client.GetFromJsonAsync<ListGymsResponse>($"{ApiEndPoints.Gyms.List}");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListGymsResponse>();
        response.Data?.Count().Should().Be(2);
    }

    [Fact]
    public async Task ListRoomsOfTheGym_ReturnCorrectRooms()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        await CreateRoom("Room-One", gym.Id);
        await CreateRoom("Room-Two", gym.Id);

        var response = await _client.GetFromJsonAsync<ListRoomsResponse>($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Rooms");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListRoomsResponse>();
        response.Data?.Count().Should().Be(2);
    }

    private async Task<RoomResponse> CreateRoom(string name, Guid gymId)
    {
        var request = new CreateRoomRequest(Name: name, Capacity: 2, GymId: gymId);

        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, request);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}");
        roomResponse.Should().NotBeNull();

        return roomResponse;
    }

    [Fact]
    public async Task ListTrainersOfTheGym_ReturnCorrectTrainers()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        var memberTrainer = await CreateMember("Trainer-One", "password123", gym.Id);
        await CreateTrainer("Trainer-One", "trainer1@gmail.com", "953000111", memberTrainer.Id);

        var memberTrainer2 = await CreateMember("Trainer-Two", "password123", gym.Id);
        await CreateTrainer("Trainer-Two", "trainer2@gmail.com", "953000112", memberTrainer2.Id);

        var response = await _client.GetFromJsonAsync<ListTrainersResponse>($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Trainers");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListTrainersResponse>();
        response.Data?.Count().Should().Be(2);
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
    public async Task ListMembersOfTheGym_ReturnCorrectMembers()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        await CreateMember("Trainer-One", "password123", gym.Id);
        await CreateMember("Trainer-Two", "password123", gym.Id);
        await CreateMember("Trainer-Three", "password123", gym.Id);

        var response = await _client.GetFromJsonAsync<ListMembersResponse>($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Members");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListMembersResponse>();
        response.Data?.Count().Should().Be(3);
    }

    [Fact]
    public async Task ListSuscriptionOfTheGym_ReturnCorrectSubscriptions()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        var member1 = await CreateMember("Member-One", "password123", gym.Id);
        await CreateSubscription(SubstriptionType.Basic, member1.Id);
        var member2 = await CreateMember("Member-Two", "password123", gym.Id);
        await CreateSubscription(SubstriptionType.Plus, member2.Id);
        var response = await _client.GetFromJsonAsync<ListSubscriptionsResponse>($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Subscriptions");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListSubscriptionsResponse>();
        response.Data?.Count().Should().Be(2);
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
    public async Task ListSessionsOfTheGym_ReturnCorrectTrainers()
    {
        var gym = await CreateGym("Gym-One", "Prenda");
        var room = await CreateRoom("Room-One", gym.Id);
        var memberTrainer = await CreateMember("Trainer-One", "password123", gym.Id);
        var trainer = await CreateTrainer("Trainer-One", "trainer1@gmail.com", "953000111", memberTrainer.Id);
        await CreateSession("Session 1", room.Id, trainer.Id);

        await CreateSession("Session 2", room.Id, trainer.Id, DateTime.Now.AddHours(3), DateTime.Now.AddHours(5));

        var response = await _client.GetFromJsonAsync<ListSessionsResponse>($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Sessions");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListSessionsResponse>();
        response.Data?.Count().Should().Be(2);
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

        var sessionResponse = await _client.GetFromJsonAsync<SessionResponse>($"Sessions/{resourceId.Id}");
        sessionResponse.Should().NotBeNull();

        return sessionResponse;
    }

    [Fact]
    public async Task ListBookingsByGym_ReturnOkWithBookings()
    {
        var gym = await CreateGym("Gym-Two", "Luanda");
        var room = await CreateRoom("Room-Two", gym.Id);
        var memberTrainer = await CreateMember("trainer.two", "password123", gym.Id);
        var trainer = await CreateTrainer("trainer-two", "trainer2@gmail.com", "953000011", memberTrainer.Id);
        var session = await CreateSession("Session 3", room.Id, trainer.Id);

        var participant1 = await CreateMember("Participante-One", "password123", gym.Id);
        var subscription1 = await CreateSubscription(SubstriptionType.Plus, participant1.Id);
        await AddRoomToSubscription(subscription1.Id, room.Id);
        await CreateBooking(session.Id, participant1.Id);

        var participant2 = await CreateMember("Participante-Two", "password123", gym.Id);
        var subscription2 = await CreateSubscription(SubstriptionType.Plus, participant2.Id);
        await AddRoomToSubscription(subscription2.Id, room.Id);
        await CreateBooking(session.Id, participant2.Id);

        // Act        
        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Gyms.Base}/{gym.Id}/Bookings");
        var response = await httpResponse.Content.ReadFromJsonAsync<ListBookingResponse>();

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Should().BeOfType<ListBookingResponse>();
        response.Data?.Count().Should().Be(2);
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