using MediatR;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using TestCommon.Gyms;
using GymManagement.Application.Gyms.Queries.Dtos;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using TestCommon.Rooms;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Rooms.Commands.CreateRoom;
using GymManagement.Domain.Gyms;
using GymManagement.Application.Rooms.Commands.UpdateRoom;
using GymManagement.Domain.Rooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Application.Sessions.Commands.CreateSession;
using GymManagement.Application.Sessions.Queries.GetSession;
using GymManagement.Application.Trainers.Queries.Dtos;
using TestCommon.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Members.Queries.Dtos;
using TestCommon.Members;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Rooms.Queries.ListRooms;
using GymManagement.Application.Rooms.Queries.ListRoomsByGym;

namespace GymManagement.Application.SubcutaneousTests.Rooms;

[Collection(MediatorFactoryCollection.CollectionName)]
public class RoomAppTests(MediatorFactory mediatorFactory): IAsyncLifetime
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    private GymDto _gym = null!;
    private RoomDto _room = null!;
    private int _roomCapacity = 2;
    
    public async Task InitializeAsync()
    {
        _gym = await CreateGym();
        _room = await CreateRoom(_gym.Id, "Room 1", _roomCapacity);
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
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

    private async Task<RoomDto> CreateRoom(Guid gymId, string? name = null, int capacity = 1)
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
    public async Task Create_WhenCommandIsValid_CreateWithSucess()
    {
        // Act
        var result = await _mediator.Send(new CreateRoomCommand("Room 2", _roomCapacity, _gym.Id));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Create_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var roomName = new string('a', nameLength);

        // Arrange 
        var command = new CreateRoomCommand(roomName, _roomCapacity, _gym.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(0)]    
    public async Task Create_WithInvalidCapacity_ReturnValidationError(int capacity)
    {
        // Arrange        
        var gym = await CreateGym("Gym-One", "Luanda");
        var command = new CreateRoomCommand("Room 2", capacity, _gym.Id);
        
        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Capacity");
        result.FirstError.Description.ToLower().Should().Contain("greater or equal to 1");
    }

    [Fact]
    public async Task Create_WhenNameAlreadyExist_ReturnValidationError()
    {
        // Arrange 
        var command = new CreateRoomCommand("Room 1", _roomCapacity, _gym.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.ToLower().Should().Contain("already exist");
    }

    [Fact]
    public async Task Create_WhenGymDontExist_ReturnNotFoundError()
    {
        var command = new CreateRoomCommand("Room 2", _roomCapacity, Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(GymErrors.GymNotFound(command.GymId));
    }

    [Fact]
    public async Task Update_WhenCommandIsValid_UpdateWithSucess()
    {
        // Act
        var command = new UpdateRoomCommand(Id: _room.Id, Name: "Room 2", Capacity: 3, _gym.Id);
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetRoomQuery(_room.Id));
        queryResult.IsError.Should().BeFalse();
        var room = queryResult.Value;

        // Assert
        room.Name.Should().BeEquivalentTo(command.Name);
        room.Capacity.Should().Be(command.Capacity);
        room.GymId.Should().Be(command.GymId);
    }

    [Fact]
    public async Task Update_WhenGymDontExist_ReturnNotFoundError()
    {
        var command = new UpdateRoomCommand(_room.Id, "Room 2", _roomCapacity, Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(GymErrors.GymNotFound(command.GymId));
    }

     [Fact]
    public async Task Update_WhenRoomDontExist_ReturnNotFoundError()
    {
        var command = new UpdateRoomCommand(Guid.NewGuid(), "Room 2", _roomCapacity, _gym.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.Id));
    }


    [Fact]
    public async Task Update_WhenOtherGymAlreadyHasTheName_ReturnValidationError()
    {
        // Arrange 
        var room = await CreateRoom(_gym.Id, "Room Two", _roomCapacity);

        // Act
        var command = new UpdateRoomCommand(Id: room.Id, Name: "Room 1", Capacity: 3, _gym.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        // result.FirstError.Code.ToLower().Should().Be("Name");
        result.FirstError.Description.ToLower().Should().Contain("name already used");
    }

    // Update no id informet, name and address validator
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Update_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var roomName = new string('a', nameLength);

        // Arrange 
        var command = new UpdateRoomCommand(_room.Id, roomName, _roomCapacity, _gym.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(0)]    
    public async Task Update_WithInvalidCapacity_ReturnValidationError(int capacity)
    {
        // Arrange        
        var command = new UpdateRoomCommand(_room.Id, "Room 2", capacity, _gym.Id);
        
        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Capacity");
        result.FirstError.Description.ToLower().Should().Contain("greater or equal to 1");
    }

    [Fact]
    public async Task Disable_WhenRoomDontExist_ReturnNotFoundError()
    {

        var command = new DisableRoomCommand(Id: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.Id));
    }

    [Fact]
    public async Task Disable_WhenRoomHasSession_ReturnCannotDisableRoomWithSessionsError()
    {
        var member = await CreateMember(_gym.Id);
        var trainer = await CreateTrainer(memberId: member.Id);
        await CreateSession(_room.Id, trainer.Id, "Session One");

        // Act
        var command = new DisableRoomCommand(Id: _room.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.CannotDisableRoomWithSessions(command.Id));        
    }

    private async Task<SessionDto> CreateSession(Guid roomId, Guid trainerId, string title)
    {
        var command = new CreateSessionCommand(roomId, trainerId, title);
        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: result.Value));
        queryResult.IsError.Should().BeFalse();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    private async Task<MemberDto> CreateMember(Guid gymId, string userName = "member1")
    {
        var createMemberCommand = MemberCommandFactory.GetCreateMemberCommand(gymId: gymId,
                                                                            userName: userName,
                                                                            password: "Abc123");
        var createMemberResult = await _mediator.Send(createMemberCommand);
        createMemberResult.IsError.Should().BeFalse();
        createMemberResult.Value.Should().NotBeEmpty();

        var queryResult = await _mediator.Send(new GetMemberQuery(Id: createMemberResult.Value));
        queryResult.IsError.Should().BeFalse();

        // Assert
        return queryResult.Value;
    }
    
     private async Task<TrainerDto> CreateTrainer(Guid memberId,
                                                 string name = "Pt1",
                                                 string phone = "923000001",
                                                 string email = "ptgym@gmail.com")
    {
        var command = TrainerCommandFactory.GetCreateTrainerCommand(memberId: memberId,
                                                                    name: name,
                                                                    phone: phone,
                                                                    email: email,
                                                                    specialization: "kickboxing");

        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: result.Value));


        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    [Fact]
    public async Task ListRooms_ReturnCorrectRooms()
    {
        // Arrange 
        var room = await CreateRoom(gymId: _gym.Id, name: "Fight Room", capacity: _roomCapacity);

        // Act
        var listResult = await _mediator.Send(new ListRoomsQuery());
        listResult.IsError.Should().BeFalse();
        var rooms = listResult.Value.ToList();

        // Assert        
        rooms.Count().Should().Be(2);
        rooms.Should().BeOfType<List<RoomDto>>();
    }    

    [Fact]
    public async Task ListRoomsByGym_ReturnCorrectRooms()
    {
        // Arrange 
        var gym = await CreateGym("Gym-Two", "Luanda");
        await CreateRoom(gymId: _gym.Id, name: "Room-2", capacity: _roomCapacity);
        await CreateRoom(gymId: _gym.Id, name: "Room-3", capacity: _roomCapacity);
        await CreateRoom(gymId:gym.Id, name: "Room-4", capacity: _roomCapacity);
        await CreateRoom(gymId: gym.Id, name: "Room-5", capacity: _roomCapacity);

        // Act
        var list1Result = await _mediator.Send(new ListRoomsByGymQuery(GymId: _gym.Id));
        list1Result.IsError.Should().BeFalse();
        var rooms1 = list1Result.Value.ToList();

        var list2Result = await _mediator.Send(new ListRoomsByGymQuery(GymId: gym.Id));
        list2Result.IsError.Should().BeFalse();
        var rooms2 = list2Result.Value.ToList();

        // Assert        
        rooms1.Count().Should().Be(3);
        rooms1.Should().BeOfType<List<RoomDto>>();

        rooms2.Count().Should().Be(2);
        rooms2.Should().BeOfType<List<RoomDto>>();
    }

}