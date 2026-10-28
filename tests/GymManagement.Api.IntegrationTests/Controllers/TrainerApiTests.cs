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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GymManagement.Api.IntegrationTests.Controllers.Trainers;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class TrainerApiTests: IAsyncLifetime
{
    private readonly HttpClient _client;
    private GymResponse _gym = null!;
    private MemberResponse _trainerMember = null!;
    private TrainerResponse _trainer = null!;

    public TrainerApiTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }
    
    public async Task InitializeAsync()
    {
        _gym = await CreateGym("Gym-One", "Adress-One");
        _trainerMember = await CreateMember("trainer-one", "Pass123", _gym.Id);
        _trainer = await CreateTrainer("Trainer-One-PT", "trainer1@gmal.com", "923000110", _trainerMember.Id);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task CreateTrainer_WhenRequestIsValid_CreateTrainer()
    {
        // Arrange
        var gym = await CreateGym("Gym-Two", "Luanda");
        var member = await CreateMember("raimundo.morais", "abc123", gym.Id);
        var request = new CreateTrainerRequest("Raimundo",
                                               "929001002",
                                               "raimundo.pt@victorygym.com",
                                               "body builder",
                                               member.Id);

        // Act
        var httpResponse = await _client.PostAsJsonAsync(ApiEndPoints.Trainers.Base, request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"{ApiEndPoints.Trainers.Base}/{resourceId.Id}");
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
    public async Task UpdateTrainer_WhenRequestIsValid_ReturnsOk()
    {
        // Arrange

        // act
        var request = new UpdateTrainerRequest("trainer-one","923000222", "trainer1.updated@gmal.com", "Body Builder");
        var httpResponse = await _client.PutAsJsonAsync($"{ApiEndPoints.Trainers.Base}/{_trainer.Id}", request);
        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();

        var response = await _client.GetFromJsonAsync<TrainerResponse>($"{ApiEndPoints.Trainers.Base}/{resourceId?.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        response.Should().BeOfType<TrainerResponse>();
        response.Name.Should().BeEquivalentTo(request.Name);
        response.Phone.Should().BeEquivalentTo(request.Phone);
        response.Email.Should().BeEquivalentTo(request.Email);
    }

    [Fact]
    public async Task Delete_WhenRequestIsValid_ReturnsNoContent()
    {       
        // act
        var httpResponse = await _client.DeleteAsync($"{ApiEndPoints.Trainers.Base}/{_trainer.Id}");

        // assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        httpResponse = await _client.GetAsync($"{ApiEndPoints.Trainers.Base}/{_trainer.Id}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        string content = await httpResponse.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
        //problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task ListTrainers_ReturnAllTrainers()
    {
        var memberTrainer2 = await CreateMember("trainer2", "Pass1234", _gym.Id);
        await CreateTrainer("trainer2", "trainer2@gmail.com", "923000333", memberTrainer2.Id);

        var httpResponse = await _client.GetAsync($"{ApiEndPoints.Trainers.List}");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await httpResponse.Content.ReadFromJsonAsync<ListTrainersResponse>();

        response.Should().NotBeNull();
        response.Should().BeOfType<ListTrainersResponse>();
        response.Data?.Count().Should().Be(2); 
    }

    [Fact]
    public async Task ListTrainerSessions_ReturnListOfSessionsByTrainer()
    {
        var room = await CreateRoom("Room-One", _gym.Id);
        await CreateSession("Morning Yoga", room.Id, _trainer.Id);
        
        var start = DateTime.Now.AddHours(2);
        await CreateSession("Night Body Build", room.Id, _trainer.Id, start, start.AddHours(1));
    
        var sessionsResponse = await _client.GetFromJsonAsync<ListSessionsResponse>($"{ApiEndPoints.Trainers.Base}/{_trainer.Id}/Sessions");

        sessionsResponse.Should().NotBeNull();
        sessionsResponse.Data?.Count().Should().Be(2);
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

}