using MediatR;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using TestCommon.Gyms;
using GymManagement.Application.Gyms.Queries.Dtos;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Commands.UpdateGym;
using GymManagement.Domain.Gyms;
using GymManagement.Application.Gyms.Commands.DeleteGym;
using TestCommon.Rooms;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Gyms.Queries.ListGyms;

namespace GymManagement.Application.SubcutaneousTests.Gyms;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GymAppTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateGym_WhenCommandIsValid_CreateWithSucess()
    {
        // Arrange 
        var createGymCommand = GymCommandFactory.GetCreateGymCommand("Kyndar", "Luanda Street");

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task CreateGym_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var gymName = new string('a', nameLength);

        // Arrange 
        var createGymCommand = GymCommandFactory.GetCreateGymCommand(name: gymName, null);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }


    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(101)]
    public async Task CreateGym_WithInvalidAddress_ReturnValidationError(int addressLength)
    {
        // Arrange 
        var address = new string('a', addressLength);
        var createGymCommand = GymCommandFactory.GetCreateGymCommand(name: "Quibuma", address);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Address");
        //result.FirstError.Description.Should().Contain("is required");
    }

    [Fact]
    public async Task CreateGym_WhenNameAlreadyExist_ReturnValidationError()
    {
        // Arrange 
        var gym = await CreateGym("Kyndar");

        // Act
        var command = GymCommandFactory.GetCreateGymCommand("Kyndar", "Luanda Street");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.ToLower().Should().Contain("already exist");
    }
    
        private async Task<GymDto> CreateGym(string? name = null, string? address = null)
    {
        var gymName = name ?? Constants.Gyms.Name;
        var gymAddress = address ?? Constants.Gyms.Address;

        var command = GymCommandFactory.GetCreateGymCommand(gymName, gymAddress);
        var createGymResult = await _mediator.Send(command);
        createGymResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetGymQuery(Id: createGymResult.Value));

        // Assert
        queryResult.IsError.Should().BeFalse();

        return queryResult.Value;
    }

    [Fact]
    public async Task Update_WhenCommandIsValid_UpdateWithSucess()
    {
        var gym = await CreateGym("Gym-One", "Luanda");

        // Act
        var command = new UpdateGymCommand(Id: gym.Id, Name: "Gym-Two", Address: "Address-Two");
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetGymQuery(gym.Id));
        queryResult.IsError.Should().BeFalse();
        gym = queryResult.Value;

        // Assert
        gym.Name.Should().BeEquivalentTo(command.Name);
        gym.Address.Should().BeEquivalentTo(command.Address);
    }

    [Fact]
    public async Task Update_WhenGymDontExist_ReturnNotFoundError()
    {
        var command = new UpdateGymCommand(Guid.NewGuid(), "Gym-Two", "Address 2");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(GymErrors.GymNotFound(command.Id));
    }

    [Fact]
    public async Task Update_WhenOtherGymAlreadyHasTheName_ReturnValidationError()
    {
        // Arrange 
        var gym = await CreateGym("Kyndar");

        var gym2 = await CreateGym("Gym-Two");
        // Act
        var command = new UpdateGymCommand(Id: gym2.Id, Name: "Kyndar", Address: "Address-Two");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.ToLower().Should().Contain("name is already used");
    }

    // Update no id informet, name and address validator, name already exist

    [Fact]
    public async Task Delete_WhenCommandIsValid_DeleteWithSucess()
    {
        var gym = await CreateGym("Gym-One", "Luanda");

        // Act
        var command = new DeleteGymCommand(Id: gym.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Delete_WhenGymDontExist_ReturnNotFoundError()
    {
        var command = new DeleteGymCommand(Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(GymErrors.GymNotFound(command.Id));
    }

    [Fact]
    public async Task Delete_WhenGymHasRooms_ReturnNotFoundError()
    {
        var gym = await CreateGym();
        var room = await CreateRoom(gym.Id, "KickBoxing", 2);

        var command = new DeleteGymCommand(gym.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(GymErrors.CannotDeleteGymWithRooms(command.Id));
    }
    
    private async Task<RoomDto> CreateRoom(Guid gymId, string name, int capacity = 1)
    {
        var command = RoomCommandFactory.GetCreateRoomCommand(gymId: gymId,
                                                           name: name, capacity: capacity);

        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        var queryResult = await _mediator.Send(new GetRoomQuery(Id: result.Value));
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    [Fact]
    public async Task ListGyms_ReturnCorrectSubscriptions()
    {
        // Arrange 
        var gym = await CreateGym();
        var gym2 = await CreateGym("Gym-Two");

        // Act
        var listResult = await _mediator.Send(new ListGymsQuery());
        listResult.IsError.Should().BeFalse();

        var gyms = listResult.Value.ToList();

        // Assert        
        gyms.Count().Should().Be(2);
        gyms.Should().BeOfType<List<GymDto>>();
    }
}