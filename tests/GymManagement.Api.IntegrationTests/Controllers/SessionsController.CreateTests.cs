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

namespace GymManagement.Api.IntegrationTests.Controllers.Sessions;

[Collection(GymManagementApiFactoryCollection.CollectionName)]
public class CreateSessionTests
{
    private readonly HttpClient _client;

    public CreateSessionTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateSession_WhenRequestIsValid_CreateSession()
    {
        // Arrange
        var gym = await CreateGym("Gym-One", "Luanda");
        var room = await CreateRoom("Room-One", gym.Id);
        var member = await CreateMember("Member 1","abc123", gym.Id);
        var trainer = await CreateTrainer("Trainer-One", "trainer1@gmail.com", member.Id);

        var request = new CreateSessionRequest(
            RoomId: room.Id,
            TrainerId: trainer.Id,
            Title: "Morning Yoga",
            StartDate: DateTime.UtcNow.AddDays(1),
            EndDate: DateTime.UtcNow.AddDays(1).AddHours(1)
        );

        // Act
        var httpResponse = await _client.PostAsJsonAsync("Sessions", request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        httpResponse.Headers.Location.Should().NotBeNull();

        var resourceId = await httpResponse.Content.ReadFromJsonAsync<ResourceIdentifier>();
        resourceId.Should().NotBeNull();
        resourceId.Id.Should().NotBeEmpty();
        httpResponse.Headers.Location!.PathAndQuery.Should().Be($"/Sessions/{resourceId.Id}");
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
}
