using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Rooms;
using GymManagement.Contracts.Sessions;
using GymManagement.Contracts.Trainers;

namespace GymManagement.Api.IntegrationTests.Controllers.Rooms;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class RoomApiTests
{
    private readonly HttpClient _client;

    public RoomApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateRoom_WhenRequestIsValid_ReturnsCreated()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");
        var request = new CreateRoomRequest(Name: "Room 1", 2, gym.Id);

        // Act
        var response = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var resourceId = await response.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        response.Headers.Location!.PathAndQuery.Should().Be($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}");

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
    public async Task UpdateRoom_WhenRequestIsValid_ReturnsOK()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Prenda");
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, new CreateRoomRequest(Name: "Room 1", 2, gym.Id));
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        // act
        var request = new UpdateRoomRequest(Id: resourceId.Id, Name: "Room-Two", Capacity: 5);
        httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Rooms.Base}/{request.Id}", request);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId!.Id}");
        roomResponse.Should().NotBeNull();
        roomResponse.Name.Should().BeEquivalentTo(request.Name);
        roomResponse.Capacity.Should().Be(request.Capacity);
    }

    [Fact]
    public async Task DisableRoom_WhenRequestIsValid_ReturnsOk()
    {
        var gym = await CreateGym("Gym-Two", "Prenda");
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, new CreateRoomRequest(Name: "Room 1", 2, gym.Id));
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();

        httpResponse = await _client.PutAsync($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}/Disable", null);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                
        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId!.Id}");
        roomResponse.Should().NotBeNull();
        roomResponse.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task ListRooms_ReturnListOfRooms()
    {
        var gym = await CreateGym("Gym-Two", "Prenda");
        await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, new CreateRoomRequest(Name: "Room 1", 2, gym.Id));
        await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Create, new CreateRoomRequest(Name: "Room 2", 2, gym.Id));

        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Rooms.List}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // string content = await httpResponse.Content.ReadAsStringAsync();
        // var roomsResponse = JsonConvert.DeserializeObject<ListRoomsResponse>(content);
        var response = await _client.GetFromJsonAsync<ListRoomsResponse>($"{ApiEndPoints.Rooms.List}");

        response.Should().NotBeNull();
        response.Should().BeOfType<ListRoomsResponse>();
        response.Data?.Count().Should().Be(2);

    }

    [Fact]
    public async Task ListRoomsSessions_ReturnListOfSessionsByRoomId()
    {
        var gym = await CreateGym("Gym-One", "Prenda");

        var room = await CreateRoom("Room-One", gym.Id);
        var memberTrainer = await CreateMember("Trainer-One", "password123", gym.Id);
        var trainer = await CreateTrainer("Trainer-One", "trainer1@gmail.com","953000111", memberTrainer.Id);
        await CreateSession("Morning Yoga", room.Id, trainer.Id);
        
        var start = DateTime.Now.AddHours(3);
        await CreateSession("Night Body Build", room.Id, trainer.Id, start, start.AddHours(1));
    
        var sessionsResponse = await _client.GetFromJsonAsync<ListSessionsResponse>($"{ApiEndPoints.Rooms.Base}/{room.Id}/Sessions");

        sessionsResponse.Should().NotBeNull();
        sessionsResponse.Data?.Count().Should().Be(2);
    }

    private async Task<RoomResponse> CreateRoom(string name, Guid gymId)
    {
        var request = new CreateRoomRequest(Name: name, Capacity: 2, GymId: gymId);

        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Rooms.Base, request);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();

        var roomResponse = await _client.GetFromJsonAsync<RoomResponse>($"{ApiEndPoints.Rooms.Base}/{resourceId.Id}");
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

    private async Task<SessionResponse> CreateSession(string title, Guid roomId, Guid trainerId, DateTime? start = null, DateTime? end = null )
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

}