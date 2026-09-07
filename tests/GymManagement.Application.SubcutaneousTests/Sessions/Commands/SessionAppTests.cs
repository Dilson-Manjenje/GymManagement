using MediatR;
using TestCommon.Subscriptions;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using TestCommon.TestConstants;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Application.Gyms.Queries.Dtos;
using TestCommon.Gyms;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Members.Queries.Dtos;
using TestCommon.Members;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Domain.Subscriptions;
using GymManagement.Application.Rooms.Queries.Dtos;
using TestCommon.Rooms;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Bookings.Commands.CreateBooking;
using GymManagement.Application.Bookings.Queries.GetBooking;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Application.Sessions.Commands.CreateSession;
using GymManagement.Application.Sessions.Queries.GetSession;
using GymManagement.Application.Trainers.Queries.Dtos;
using TestCommon.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using TestCommon.Sessions;
using GymManagement.Domain.Sessions;
using ErrorOr;
using GymManagement.Application.Sessions.Shared;
using Humanizer;
using GymManagement.Domain.Rooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Domain.Trainers;

namespace GymManagement.Application.SubcutaneousTests.Sessions;

[Collection(MediatorFactoryCollection.CollectionName)]
public class SessionAppTests(MediatorFactory mediatorFactory):  IAsyncLifetime
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private GymDto _gym = null!;
    private MemberDto _trainerMember = null!;
    private MemberDto _participant = null!;
    private TrainerDto _trainer = null!;
    private RoomDto _room = null!;
    private int _roomCapacity = 2;
    private SubscriptionDto _subscription = null!;
    private SessionDto _session = null!;
    private string _title = "Morning Session 1";

    public async Task InitializeAsync()
    {
        _gym = await CreateGym();
        _trainerMember = await CreateMember(_gym.Id,"Trainer 1");
        _trainer = await CreateTrainer(_trainerMember.Id);
        _participant = await CreateMember(_gym.Id, "Participant One");
        _room = await CreateRoom(_gym.Id, "Room 1", _roomCapacity);
        _subscription = await CreateSubscription(SubscriptionType.Plus, _participant.Id);
        //_session = await CreateSession(_room.Id, _trainer.Id, "Title");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateSession_WhenCommandIsValid_CreateActiveSession()
    {
        // Arrange 
        
        // Act
        var command = SessionCommandFactory.GetCreateSessionCommand(roomId: _room.Id, trainerId: _trainer.Id, "Session 1");
        var result = await _mediator.Send(command);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        var query = new GetSessionQuery(Id: result.Value);
        var queryResult = await _mediator.Send(query);
        queryResult.IsError.Should().BeFalse();
        var session = queryResult.Value;

        // Assert
        session.Status.Should().Be(SessionStatus.Scheduled);
        session.Id.Should().NotBeEmpty();
        session.Vacancy.Should().Be(_room.Capacity);
        // session.EndDate.Should().Be(session.StartDate.AddDays(session.SubscriptionType.DurationInDays));
    }
    
    private async Task<GymDto> CreateGym(string? name = null)
    {
        var gymName = name ?? Constants.Gyms.Name;

        var createGymCommand = GymCommandFactory.GetCreateGymCommand(gymName, Constants.Gyms.Address);
        var createGymResult = await _mediator.Send(createGymCommand);

        var queryResult = await _mediator.Send(new GetGymQuery(Id: createGymResult.Value));

        // Assert
        createGymResult.IsError.Should().BeFalse();
        createGymResult.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    private async Task<MemberDto> CreateMember(Guid gymId, string userName = "member1")
    {
        var createMemberCommand = MemberCommandFactory.GetCreateMemberCommand(gymId: gymId,
                                                                            userName: userName,
                                                                            password: "Abc123");
        var createMemberResult = await _mediator.Send(createMemberCommand);

        var queryResult = await _mediator.Send(new GetMemberQuery(Id: createMemberResult.Value));


        // Assert
        createMemberResult.IsError.Should().BeFalse();
        createMemberResult.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    private async Task<SubscriptionDto> CreateSubscription(SubscriptionType type, Guid memberId)
    {
        var command = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: type, memberId: memberId);

        var result = await _mediator.Send(command);

        // Ensure create sucessfully
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSubscriptionQuery(Id: result.Value));
        result.IsError.Should().BeFalse();

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
    
    private async Task<TrainerDto> CreateTrainer(Guid memberId)
    {
        var command = TrainerCommandFactory.GetCreateTrainerCommand(memberId: memberId,
                                                                    name: "Pt1",
                                                                    phone: "923000001",
                                                                    email: "ptgym@gmail.com",
                                                                    specialization: "kickboxing");

        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: result.Value));


        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    [Fact]
    public async Task CreateSession_WithEmptyTitle_ReturnValidationError()
    {
        var command = new CreateSessionCommand(_room.Id, _trainer.Id, "");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Title");
        result.FirstError.Description.Should().Contain("required");
    }

    [Fact]
    public async Task CreateSession_WithTitleLessThanRequired_ReturnValidationError()
    {
        var command = new CreateSessionCommand(_room.Id, _trainer.Id, "abc");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Title");
        result.FirstError.Description.Should().Contain("must have at least");
    }

    [Fact]
    public async Task CreateSession_WithTitleMoreThanAllowed_ReturnValidationError()
    {
        string title = new('a', 61);
        var command = new CreateSessionCommand(_room.Id, _trainer.Id, title);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Title");
        result.FirstError.Description.Should().Contain("must have at most");
    }

    [Fact]
    public async Task CreateSession_WhenStartDateLessThanNow_ReturnValidationError()
    {
        DateTime startDate = DateTime.Now.AddHours(-2);

        var command = new CreateSessionCommand(_room.Id, _trainer.Id, _title, startDate);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("StartDate");
        result.FirstError.Description.Should().Contain("must be greater than now");

    }

    [Fact]
    public async Task CreateSession_WhenStartDateGreatherThanEndDate_ReturnValidationError()
    {
        DateTime startDate = DateTime.Now.AddDays(5);
        DateTime endDate = DateTime.Now;

        var command = new CreateSessionCommand(_room.Id, _trainer.Id, _title, startDate, endDate);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("StartDate");
        result.FirstError.Description.Should().Contain("must be less than");
    }

    [Fact]
    public async Task CreateSession_WhenStartAfterLocalBussinesHour_ReturnValidationError()
    {
        DateTime startDate = DateTime.Now.At(23);

        var command = new CreateSessionCommand(_room.Id, _trainer.Id, _title, startDate);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("StartDate");
        result.FirstError.Description.Should().Contain("at or before 22:00");
    }

    // Test Cases: room/trainer dont exist, room is not available, room and trainer not in same gym, 
    // Room has overlappingSession

    [Fact]
    public async Task CreateSession_WhenRoomDontExist_ReturnRoomNotFoundError()
    {
        var command = new CreateSessionCommand(Guid.NewGuid(), _trainer.Id, _title);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.RoomId));
    }

    [Fact]
    public async Task CreateSession_RoomIsNotAvailable_ReturnRoomIsNotIsAvailableError()
    {
        var disableRoomCmd = new DisableRoomCommand(_room.Id);
        var disableRoomResult = await _mediator.Send(disableRoomCmd);
        disableRoomResult.IsError.Should().BeFalse();

        var command = new CreateSessionCommand(_room.Id, _trainer.Id, _title);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomIsNotIsAvailable(command.RoomId));
    }

    [Fact]
    public async Task CreateSession_WhenTrainerDontExist_ReturnTrainerNotFoundError()
    {
        var command = new CreateSessionCommand(_room.Id, Guid.NewGuid(), _title);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerNotFound(command.TrainerId));
    }

    [Fact]
    public async Task CreateSession_WhenTrainerAndRoomNotInSameGym_ReturnTrainerNotInTheSameGymError()
    {
        var gym = await CreateGym("Gym Two");
        var anotherRoom = await CreateRoom(gym.Id, "Room Two", 2);

        var command = new CreateSessionCommand(anotherRoom.Id, _trainer.Id, _title);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.TrainerNotInTheSameGym(command.TrainerId));
    }

    [Fact]
    public async Task CreateSession_WhenRoomIsSessionOnSameTime_ReturnRoomHasOverlappingSessionError()
    {
        DateTime startDate = DateTime.Now.AddMinutes(10);

        var createSession1Cmd = new CreateSessionCommand(_room.Id, _trainer.Id, _title, startDate);
        var result1 = await _mediator.Send(createSession1Cmd);
        result1.IsError.Should().BeFalse();

        var createSession2Cmd = new CreateSessionCommand(_room.Id, _trainer.Id, _title, startDate.AddHours(1));
        var result2 = await _mediator.Send(createSession2Cmd);

        // Assert
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(RoomErrors.RoomHasOverlappingSession());
    }

    private async Task<BookingDto> CreateBooking(Guid sessionId, Guid memberId)
    {
        var command = new CreateBookingCommand(SessionId: sessionId, MemberId: memberId);

        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: result.Value));

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }
    
    private async Task<SessionDto> CreateSession(Guid roomId, Guid trainerId, string title)
    {
        var command = new CreateSessionCommand(roomId, trainerId, title);
        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: result.Value));


        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }
    
   


}