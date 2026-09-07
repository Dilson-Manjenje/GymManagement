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
using GymManagement.Domain.Sessions;
using ErrorOr;
using Humanizer;
using GymManagement.Domain.Rooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Domain.Trainers;
using GymManagement.Application.Sessions.Commands.DeleteSession;
using GymManagement.Application.Sessions.Commands.CancelSession;
using GymManagement.Application.Sessions.Commands.FinalizeSession;
using GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;
using GymManagement.Application.Sessions.Commands.UpdateSession;

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
        _session = await CreateSession(_room.Id, _trainer.Id, _title);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void CreateSession_WhenCommandIsValid_CreateActiveSession()
    {
        var session = _session;

        // Assert
        session.Status.Should().Be(SessionStatus.Scheduled);
        session.Id.Should().NotBeEmpty();
        session.Vacancy.Should().Be(_room.Capacity);
        session.Capacity.Should().Be(_room.Capacity);
        session.EndDate.Should().Be(_session.StartDate.AddHours(2));

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
        var room = await CreateRoom(_gym.Id, "Room 2", 2);

        var disableRoomCmd = new DisableRoomCommand(room.Id);
        var disableRoomResult = await _mediator.Send(disableRoomCmd);
        disableRoomResult.IsError.Should().BeFalse();

        var command = new CreateSessionCommand(room.Id, _trainer.Id, _title);
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
    public async Task CreateSession_WhenRoomHasStartDateOverlaping_ReturnRoomHasOverlappingSessionError()
    {
        var room = await CreateRoom(_gym.Id, "Room 2", 2);
        
        DateTime startDate = DateTime.Now.AddMinutes(10);

        var createSession1Cmd = new CreateSessionCommand(room.Id, _trainer.Id, _title, startDate);
        var result1 = await _mediator.Send(createSession1Cmd);
        result1.IsError.Should().BeFalse();

        var createSession2Cmd = new CreateSessionCommand(room.Id, _trainer.Id, _title, startDate.AddHours(1));
        var result2 = await _mediator.Send(createSession2Cmd);

        // Assert
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(RoomErrors.RoomHasOverlappingSession());
    }

    [Fact]
    public async Task CreateSession_WhenRoomHasEndDateOverlaping_ReturnRoomHasOverlappingSessionError()
    {
        var room = await CreateRoom(_gym.Id, "Room 2", 2);

        DateTime startDate = DateTime.Now.AddMinutes(10); // 9
        DateTime endDate = startDate.AddHours(2); // 11

        var createSession1Cmd = new CreateSessionCommand(room.Id, _trainer.Id, _title, startDate, endDate);
        var result1 = await _mediator.Send(createSession1Cmd);
        result1.IsError.Should().BeFalse();

        DateTime secondStartDate = startDate.AddHours(1); // 10 
        DateTime secondEndDate = endDate.AddHours(1); // 12 
        var createSession2Cmd = new CreateSessionCommand(room.Id, _trainer.Id, _title, secondStartDate, secondEndDate);
        var result2 = await _mediator.Send(createSession2Cmd);

        // Assert
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(RoomErrors.RoomHasOverlappingSession());
    }

    [Fact]
    public async Task DeleteSession_WhenSessionDontHaveBooking_DeleteWithSucess()
    {
        var command = new DeleteSessionCommand(Id: _session.Id);
        var cmdResult = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));

        // Assert
        cmdResult.IsError.Should().BeFalse();
        cmdResult.Value.Should().Be(Unit.Value);
        queryResult.IsError.Should().BeTrue();
        queryResult.FirstError.Should().Be(SessionErrors.SessionNotFound(_session.Id));
    }

    [Fact]
    public async Task DeleteSession_WhenSessionDontExist_ReturnNotFoundError()
    {
        var command = new DeleteSessionCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.SessionNotFound(command.Id));
    }

    [Fact]
    public async Task DeleteSession_WhenSessionIsCanceled_ReturnCantChangeSessionError()
    {        

        var cancelCmd = new CancelSessionCommand(Id: _session.Id);
        var cancelResult = await _mediator.Send(cancelCmd);
        cancelResult.IsError.Should().BeFalse();

        var deleteCmd = new DeleteSessionCommand(Id: _session.Id);
        var deleteResult = await _mediator.Send(deleteCmd);

        // Assert
        deleteResult.IsError.Should().BeTrue();
        deleteResult.FirstError.Should().Be(SessionErrors.CantChangeSession(deleteCmd.Id));
    }

    [Fact]
    public async Task DeleteSession_WhenSessionIsFinalized_ReturnCantChangeSessionError()
    {       
        var finalizeCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizeCmd);
        finalizeResult.IsError.Should().BeFalse();

        var deleteCmd = new DeleteSessionCommand(Id: _session.Id);
        var deleteResult = await _mediator.Send(deleteCmd);

        // Assert
        deleteResult.IsError.Should().BeTrue();
        deleteResult.FirstError.Should().Be(SessionErrors.CantChangeSession(deleteCmd.Id));
    }

    [Fact]
    public async Task DeleteSession_WithBooking_ReturnCantDeleteSessionWithBookingError()
    {
        var addRoomCmd = new AddRoomToSubscriptionCommand(_subscription.Id, _room.Id);
        var addRoomResult = await _mediator.Send(addRoomCmd);
        addRoomResult.IsError.Should().BeFalse();

        var booking = await CreateBooking(_session.Id, _participant.Id);

        var deleteCmd = new DeleteSessionCommand(Id: _session.Id);
        var deleteResult = await _mediator.Send(deleteCmd);

        // Assert
        deleteResult.IsError.Should().BeTrue();
        deleteResult.FirstError.Should().Be(SessionErrors.CantCancelOrDeleteSessionWithBooking(deleteCmd.Id));
    }

    [Fact]
    public async Task FinalizeSession_WhenSessionIsActive_FinalizeWithSucess()
    {
        var finalizeCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizeCmd);
        finalizeResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        queryResult.IsError.Should().BeFalse();
        
        var session = queryResult.Value;

        // Assert
        session.Status.Should().Be(SessionStatus.Finalized);
    }

    [Fact]
    public async Task FinalizeSession_WhenSessionDontExist_ReturnNotFoundError()
    {
        var command = new FinalizeSessionCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.SessionNotFound(command.Id));
    }

    [Fact]
    public async Task FinalizeSession_WhenSessionIsCanceled_ReturnCantChangeSessionError()
    {
        var cancelCmd = new CancelSessionCommand(Id: _session.Id);
        var cancelResult = await _mediator.Send(cancelCmd);
        cancelResult.IsError.Should().BeFalse();

        var finalizeCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizeCmd);

        // Assert
        finalizeResult.IsError.Should().BeTrue();
        finalizeResult.FirstError.Should().Be(SessionErrors.CantChangeSession(finalizeCmd.Id));
    }

    [Fact]
    public async Task FinalizeSession_WhenSessionIsFinalized_ReturnCantChangeSessionError()
    {
        var finalizedCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizedCmd);
        finalizeResult.IsError.Should().BeFalse();

        var cmd = new FinalizeSessionCommand(Id: _session.Id);
        var result = await _mediator.Send(cmd);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(cmd.Id));
    }
    
    [Fact]
    public async Task CancelSession_WhenSessionIsActive_FinalizeWithSucess()
    {

        var cancelCmd = new CancelSessionCommand(Id: _session.Id);
        var cancelResult = await _mediator.Send(cancelCmd);
        cancelResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        queryResult.IsError.Should().BeFalse();
        
        var session = queryResult.Value;

        // Assert
        session.Status.Should().Be(SessionStatus.Canceled);
    }

    [Fact]
    public async Task CancelSession_WhenSessionDontExist_ReturnNotFoundError()
    {
        var command = new CancelSessionCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.SessionNotFound(command.Id));
    }

    [Fact]
    public async Task CancelSession_WhenSessionIsCanceled_ReturnCantChangeSessionError()
    {
        var cancelCmd = new CancelSessionCommand(Id: _session.Id);
        var cancelResult = await _mediator.Send(cancelCmd);
        cancelResult.IsError.Should().BeFalse();

        var command = new CancelSessionCommand(Id: _session.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(command.Id));
    }

    [Fact]
    public async Task CancelSession_WhenSessionIsFinalized_ReturnCantChangeSessionError()
    {
        var finalizeCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizeCmd);
        finalizeResult.IsError.Should().BeFalse();

        var command = new CancelSessionCommand(Id: _session.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(command.Id));
    }

    [Fact]
    public async Task UpdateSession_WhenCommandIsValid_UpdateWithSucess()
    {
        var newRoom = await CreateRoom(_gym.Id, "Room 2", 2);
        var member = await CreateMember(_gym.Id, "UpdateMember");
        var newTrainer = await CreateTrainer(member.Id, "Pt2", "923001001", "pt2@gmail.com");

        var command = new UpdateSessionCommand(Id: _session.Id, newRoom.Id, newTrainer.Id, _title);
        var cmdResult = await _mediator.Send(command);
        cmdResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        queryResult.IsError.Should().BeFalse();
        var session = queryResult.Value;

        // Assert        
        session.RoomId.Should().Be(newRoom.Id);
        session.TrainerId.Should().Be(newTrainer.Id);
        session.Title.Should().Be(_title);
    }


    [Fact]
    public async Task UpdateSession_WhenSessionDontExist_ReturnNotFoundError()
    {
        var command = new UpdateSessionCommand(Id: Guid.NewGuid(), _room.Id, _trainer.Id, _title);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.SessionNotFound(command.Id));
    }

    [Fact]
    public async Task UpdateSession_WhenRoomDontExist_ReturnNotFoundError()
    {
        var command = new UpdateSessionCommand(Id: _session.Id, Guid.NewGuid(), _trainer.Id, _title);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.RoomId));
    }
    
    [Fact]
    public async Task UpdateSession_WhenTrainerDontExist_ReturnNotFoundError()
    {
        var command = new UpdateSessionCommand(Id: _session.Id, _room.Id, Guid.NewGuid(), _title);
        
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerNotFound(command.TrainerId));
    }

    
    [Fact]
    public async Task UpdateSession_WhenSessionIsCanceled_ReturnCantChangeSessionError()
    {       
        var cancelCmd = new CancelSessionCommand(Id: _session.Id);
        var cancelResult = await _mediator.Send(cancelCmd);
        cancelResult.IsError.Should().BeFalse();

        var updateCmd = new UpdateSessionCommand(Id: _session.Id, _room.Id, _trainer.Id, "Session 2");
        var updateResult = await _mediator.Send(updateCmd);

        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.FirstError.Should().Be(SessionErrors.CantChangeSession(updateCmd.Id));
    }

    [Fact]
    public async Task UpdateSession_WhenSessionIsFinalized_ReturnCantChangeSessionError()
    {
        var finalizeCmd = new FinalizeSessionCommand(Id: _session.Id);
        var finalizeResult = await _mediator.Send(finalizeCmd);
        finalizeResult.IsError.Should().BeFalse();

        var updateCmd = new UpdateSessionCommand(Id: _session.Id, _room.Id, _trainer.Id, "Session 2");
        var updateResult = await _mediator.Send(updateCmd);

        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.FirstError.Should().Be(SessionErrors.CantChangeSession(updateCmd.Id));
    }

    [Fact]
    public async Task Update_WhenRoomHasDifferentGym_ReturnCantChangeGymError()
    {
        var gym = await CreateGym("Gym Two");
        var anotherRoom = await CreateRoom(gym.Id, "Room Two", 2);

        var updateCmd = new UpdateSessionCommand(Id: _session.Id, anotherRoom.Id, _trainer.Id, "Session 2");
        var result = await _mediator.Send(updateCmd);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeGym());
    }

    [Fact]
    public async Task Update_WhenTrainerHasDifferentGym_ReturnCantChangeGymError()
    {
        var gym = await CreateGym("Gym Two");
        var member = await CreateMember(gym.Id, "member2");
        var newTrainer = await CreateTrainer(member.Id, "Pt2", "923001001", "pt2@gmail.com");

        var updateCmd = new UpdateSessionCommand(Id: _session.Id, _room.Id, newTrainer.Id, "Session 2");
        var result = await _mediator.Send(updateCmd);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.TrainerNotInTheSameGym(updateCmd.TrainerId));
    }
    
    [Fact]
    public async Task Update_WhenRoomHasStartDateOverlaping_ReturnRoomHasOverlappingSessionError()
    {
        var room = await CreateRoom(_gym.Id, "Room 2", 2);        
        DateTime startDate = DateTime.Now.AddMinutes(10);

        var updateCmd = new UpdateSessionCommand(Id: _session.Id, _room.Id, _trainer.Id, _title, startDate);
        var updateResult = await _mediator.Send(updateCmd);
        
        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.FirstError.Should().Be(RoomErrors.RoomHasOverlappingSession());
    }

    [Fact]
    public async Task UpdateSession_WhenRoomHasEndDateOverlaping_ReturnRoomHasOverlappingSessionError()
    {
        DateTime startDate = DateTime.Now.AddMinutes(10); // 9
        DateTime endDate = startDate.AddHours(2); // 11
        
        var updateCmd = new UpdateSessionCommand(Id: _session.Id, _room.Id, _trainer.Id, _title, startDate, endDate);
        var updateResult = await _mediator.Send(updateCmd);
        
        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.FirstError.Should().Be(RoomErrors.RoomHasOverlappingSession());
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
        queryResult.IsError.Should().BeFalse();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }   
}