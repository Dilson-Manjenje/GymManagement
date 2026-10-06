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
using GymManagement.Application.Sessions.Commands.CancelSession;
using GymManagement.Application.Sessions.Commands.FinalizeSession;
using GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Members;
using GymManagement.Application.Subscriptions.Commands.DisableSubscription;
using GymManagement.Application.Bookings.Commands.CancelBooking;
using GymManagement.Application.Bookings.Commands.FinalizeBooking;
using GymManagement.Application.Bookings.Queries.ListBookings;
using GymManagement.Application.Bookings.Queries.ListBookingsByGym;
using GymManagement.Application.Bookings.Queries.ListBookingsByMember;
using GymManagement.Application.Bookings.Queries.ListBookingsBySession;

namespace GymManagement.Application.SubcutaneousTests.Bookings;

[Collection(MediatorFactoryCollection.CollectionName)]
public class BookingAppTests(MediatorFactory mediatorFactory) : IAsyncLifetime
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
    private BookingDto _booking = null!;
    private string _title = "Morning Session 1";

    public async Task InitializeAsync()
    {
        _gym = await CreateGym();
        _trainerMember = await CreateMember(_gym.Id, "Trainer 1");
        _trainer = await CreateTrainer(_trainerMember.Id);
        _participant = await CreateMember(_gym.Id, "Participant One");
        _room = await CreateRoom(_gym.Id, "Room 1", _roomCapacity);
        _subscription = await CreateSubscription(SubscriptionType.Plus, _participant.Id);
        _session = await CreateSession(_room.Id, _trainer.Id, _title);
        _booking = await CreateBooking(_session.Id, _participant.Id);

    }

    public Task DisposeAsync() => Task.CompletedTask;

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

    private async Task<BookingDto> CreateBooking(Guid sessionId, Guid memberId, Guid? roomId = null, Guid? subscriptionId = null)
    {
        Guid selectedRoomId = roomId is null ? _room.Id : roomId.Value;
        Guid selectedSubscriptionId = subscriptionId is null ? _subscription.Id : subscriptionId.Value;

        var addRoomResult = await _mediator.Send(new AddRoomToSubscriptionCommand(selectedSubscriptionId, selectedRoomId));
        addRoomResult.IsError.Should().BeFalse();

        var createBookingResult = await _mediator.Send(new CreateBookingCommand(SessionId: sessionId, MemberId: memberId));
        createBookingResult.IsError.Should().BeFalse();
        createBookingResult.Value.Should().NotBeEmpty();

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: createBookingResult.Value));
        queryResult.IsError.Should().BeFalse();

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

    [Fact]
    public void Create_WhenSubscriptionHaveTheRoom_CreateActiveBooking()
    {
        _booking.Status.Should().Be(BookingStatus.Active);
        _session.Capacity.Should().Be(_roomCapacity);
    }

    [Fact]
    public async Task Create_WhenSubscriptionDontHaveTheRoom_ReturnSubscriptionDontHaveAccessError()
    {
        var room = await CreateRoom(_gym.Id, "Room 2", 2);
        var session = await CreateSession(room.Id, _trainer.Id, "Session 2");

        var result = await _mediator.Send(new CreateBookingCommand(SessionId: session.Id, MemberId: _participant.Id));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.SubscriptionDontHaveAccess(subscriptionId: _subscription.Id, roomId: session.RoomId));
    }

    [Fact]
    public async Task Create_WhenSessionDontExist_ReturnNotFoundError()
    {

        var command = new CreateBookingCommand(SessionId: Guid.NewGuid(), MemberId: _participant.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.SessionNotFound(command.SessionId));
    }

    [Fact]
    public async Task Create_WhenMemberDontExist_ReturnNotFoundError()
    {

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound(command.MemberId));
    }

    [Fact]
    public async Task Create_WhenMemberDontHaveSubscription_ReturnMemberDontHaveActiveSubscriptionError()
    {
        var member = await CreateMember(_gym.Id, "Member 2");

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: member.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.MemberDontHaveActiveSubscription(command.MemberId));
    }

    [Fact]
    public async Task Create_WhenMemberSubscriptionExpired_ReturnMemberDontHaveActiveSubscriptionError()
    {

        var disableSubsResult = await _mediator.Send(new DisableSubscriptionCommand(_subscription.Id));
        disableSubsResult.IsError.Should().BeFalse();
        
        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: _participant.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.MemberDontHaveActiveSubscription(command.MemberId));
    }

    [Fact]
    public async Task Create_WhenMemberAlreadyBookedInSession_ReturnDuplicateBookingError()
    {

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: _participant.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.DuplicateBooking(command.MemberId, command.SessionId));
    }

    [Fact]
    public async Task Create_WhenMemberIsNotInSameGym_ReturnMemberNotInTheSameGymError()
    {
        var gym = await CreateGym("Gym Two");
        var member = await CreateMember(gym.Id, "Member Two");
        var subscription = await CreateSubscription(SubscriptionType.Plus, member.Id);

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: member.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.MemberNotInTheSameGym(command.MemberId));
    }

    [Fact]
    public async Task Create_WhenSessionIsCanceled_ReturnInvalidSessionsStatusError()
    {
        var cancelResult = await _mediator.Send(new CancelSessionCommand(Id: _session.Id));
        cancelResult.IsError.Should().BeFalse();

        var getSessionResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        getSessionResult.IsError.Should().BeFalse();
        var session = getSessionResult.Value;

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: _participant.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(session.Id, statusName: session.Status.Name));
    }

    [Fact]
    public async Task Create_WhenSessionIsFinalized_ReturnInvalidSessionsStatusError()
    {
        var finalizeResult = await _mediator.Send(new FinalizeSessionCommand(Id: _session.Id));
        finalizeResult.IsError.Should().BeFalse();

        var getSessionResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        getSessionResult.IsError.Should().BeFalse();
        var session = getSessionResult.Value;


        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: _participant.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(session.Id, statusName: session.Status.Name));
    }

    [Fact]
    public async Task Create_WhenSessionIsFull_ReturnCannotExceedSessionCapacityError()
    {
        var member2 = await CreateMember(_gym.Id, "Member Two");
        var member3 = await CreateMember(_gym.Id, "Member Three");
        var subscription2 = await CreateSubscription(SubscriptionType.Plus, member2.Id);
        var subscription3 = await CreateSubscription(SubscriptionType.Plus, member3.Id);

        var addRoomResult2 = await _mediator.Send(new AddRoomToSubscriptionCommand(subscription2.Id, _room.Id));
        addRoomResult2.IsError.Should().BeFalse();

        var bookingResult2 = await _mediator.Send(new CreateBookingCommand(SessionId: _session.Id, MemberId: member2.Id));
        bookingResult2.IsError.Should().BeFalse();

        var addRoomResult3 = await _mediator.Send(new AddRoomToSubscriptionCommand(subscription3.Id, _room.Id));
        addRoomResult3.IsError.Should().BeFalse();

        var command = new CreateBookingCommand(SessionId: _session.Id, MemberId: member3.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CannotExceedSessionCapacity);
    }

    [Fact]
    public async Task CreateBooking_DecrementSessionVacancy_SetVacancyCorrectly()
    {
        // This test case validate the side effects of BookingCreatedEventHandler
        // Which is responsable for Decrement Vacancy
        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        queryResult.IsError.Should().BeFalse();

        var session = queryResult.Value;
        var expected = _session.Vacancy - 1;  // Validate BookingCreatedEventHandler

        // Assert
        session.Capacity.Should().Be(_roomCapacity);              
        session.Vacancy.Should().Be(expected);

    }


    [Fact]
    public async Task Cancel_WhenBookingExist_CancelWithSucess()
    {
        var result = await _mediator.Send(new CancelBookingCommand(Id: _booking.Id));
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: result.Value));
        queryResult.IsError.Should().BeFalse();
        
        var booking = queryResult.Value;

        // Assert
        booking.Status.Should().Be(BookingStatus.Canceled);        
    }

    [Fact]
    public async Task Cancel_WhenBookingDontExist_ReturnNotFoundError()
    {

        var command = new CancelBookingCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.BookingNotFound(command.Id));
    }

    [Fact]
    public async Task Cancel_WhenBookingIsCanceled_ReturnCantChangeBookingError()
    {
        var command = new CancelBookingCommand(Id: _booking.Id);

        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var canceledResult = await _mediator.Send(new CancelBookingCommand(Id: command.Id));

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: command.Id));
        queryResult.IsError.Should().BeFalse();
        var booking = queryResult.Value;

        // Assert
        canceledResult.IsError.Should().BeTrue();
        canceledResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(id: command.Id, statusName: booking.Status.Name));

    }

    [Fact]
    public async Task Cancel_WhenBookingIsFinalized_ReturnCantChangeBookingError()
    {
        var finalizeSessonResult = await _mediator.Send(new FinalizeSessionCommand(Id: _session.Id));
        finalizeSessonResult.IsError.Should().BeFalse();

        var cancelBookingResult = await _mediator.Send(new CancelBookingCommand(Id: _booking.Id));
        cancelBookingResult.IsError.Should().BeTrue();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _booking.SessionId));
        queryResult.IsError.Should().BeFalse();
        var session = queryResult.Value;

        // Assert
        cancelBookingResult.IsError.Should().BeTrue();
        cancelBookingResult.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(_session.Id, statusName: session.Status.Name));
    }

    [Fact]
    public async Task Cancel_WhenSessionIsCanceled_ReturnCantChangeBookingError()
    {
        var cancelSessonResult = await _mediator.Send(new CancelSessionCommand(Id: _session.Id));
        cancelSessonResult.IsError.Should().BeFalse();

        var cancelBookingResult = await _mediator.Send(new CancelBookingCommand(Id: _booking.Id));
        cancelBookingResult.IsError.Should().BeTrue();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _booking.SessionId));
        queryResult.IsError.Should().BeFalse();
        var session = queryResult.Value;

        // Assert
        cancelBookingResult.IsError.Should().BeTrue();
        cancelBookingResult.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(_session.Id, statusName: session.Status.Name));
    }

    [Fact]
    public async Task CancelBooking_IncrementSessionVacancy_SetVacancyCorrectly()
    {
        // This validate the side effects of BookingCanceledEventHandler
        // Which is responsable for Increment Vacancy
        var cancelBookingResult = await _mediator.Send(new CancelBookingCommand(Id: _booking.Id));
        cancelBookingResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _session.Id));
        queryResult.IsError.Should().BeFalse();

        var session = queryResult.Value;

        // Assert
        session.Capacity.Should().Be(_roomCapacity);
        session.Vacancy.Should().Be(_roomCapacity);

    }
    
    [Fact(DisplayName = "FinalizeBooking_WhenSessionCompleted")]
    public async Task FinalizeBooking_WhenSessionIsCompleted_FinalizeWithSucess()
    {
        // This validate the side effects of SessionFinalizedEventHandler
        // Which is responsable for Finalize the Bookings
        var finalizeSessionResult = await _mediator.Send(new FinalizeSessionCommand(Id: _session.Id));
        finalizeSessionResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: _booking.Id));
        queryResult.IsError.Should().BeFalse();

        var booking = queryResult.Value;

        // Assert
        booking.Status.Should().Be(BookingStatus.Finalized);
    }

    [Fact]
    public async Task Finalize_WhenBookingDontExist_ReturnNotFoundError()
    {

        var command = new FinalizeBookingCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(BookingErrors.BookingNotFound(command.Id));
    }

    [Fact]
    public async Task Finalize_WhenSessionIsNotFinalized_ReturnCantChangeBookingError()
    {
        var command = new FinalizeBookingCommand(Id: _booking.Id);
        var finalizeResult = await _mediator.Send(new FinalizeBookingCommand(Id: command.Id));

        var queryResult = await _mediator.Send(new GetSessionQuery(Id: _booking.SessionId));
        queryResult.IsError.Should().BeFalse();
        var session = queryResult.Value;

        // Assert
        finalizeResult.IsError.Should().BeTrue();
        finalizeResult.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(id: _session.Id, statusName: session.Status.Name));
    }

    [Fact]
    public async Task Finalize_WhenBookingIsCanceled_ReturnCantChangeBookingError()
    {
        var cancelResult = await _mediator.Send(new CancelBookingCommand(Id: _booking.Id));
        cancelResult.IsError.Should().BeFalse();

        var finalizeSessonResult = await _mediator.Send(new FinalizeSessionCommand(Id: _session.Id));
        finalizeSessonResult.IsError.Should().BeFalse();

        var finalizeBookingResult = await _mediator.Send(new FinalizeBookingCommand(Id: _booking.Id));
        finalizeBookingResult.IsError.Should().BeTrue();

        var queryResult = await _mediator.Send(new GetBookingQuery(Id: _booking.Id));
        queryResult.IsError.Should().BeFalse();
        var booking = queryResult.Value;

        // Assert        
        finalizeBookingResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(id: booking.Id, statusName: booking.Status.Name));
    }

    [Fact]
    public async Task ListBookings_ReturnCorrectBookings()
    {
        var member = await CreateMember(_gym.Id, "Member 2");
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        await CreateBooking(_session.Id, member.Id, _room.Id, subscription.Id);

        var listResult = await _mediator.Send(new ListBookingsQuery());
        listResult.IsError.Should().BeFalse();

        var bookings = listResult.Value.ToList();

        // Assert        
        bookings.Count().Should().Be(2);
        bookings.Should().BeOfType<List<BookingDto>>();
    }

    [Fact]
    public async Task ListBookingsByGym_ReturnCorrectBookings()
    {
        var gym2 = await CreateGym("Gym Two");
        var room2 = await CreateRoom(gym2.Id, "Room 2");
        var memberPt = await CreateMember(gym2.Id, "Pt-Member");
        var trainer2 = await CreateTrainer(memberPt.Id, "PT2", "923000033", "PT22@gmail.com");
        var member2 = await CreateMember(gym2.Id, "Member 2");
        var subscription2 = await CreateSubscription(SubscriptionType.Basic, member2.Id);
        var session2 = await CreateSession(room2.Id, trainer2.Id, "Session 2");
        var booking2 = await CreateBooking(session2.Id, member2.Id, room2.Id, subscription2.Id);


        var member3 = await CreateMember(_gym.Id, "Member 3");
        var subscription3 = await CreateSubscription(SubscriptionType.Basic, member3.Id);
        var booking = await CreateBooking(_session.Id, member3.Id, _room.Id, subscription3.Id);

        var listResult = await _mediator.Send(new ListBookingsByGymQuery(GymId: _gym.Id));
        listResult.IsError.Should().BeFalse();
        var bookings1 = listResult.Value.ToList();

        var list2Result = await _mediator.Send(new ListBookingsByGymQuery(GymId: gym2.Id));
        list2Result.IsError.Should().BeFalse();
        var bookings2 = list2Result.Value.ToList();

        // Assert        
        bookings1.Count().Should().Be(2);
        bookings1.Should().BeOfType<List<BookingDto>>();
        bookings2.Count().Should().Be(1);
        bookings2.Should().BeOfType<List<BookingDto>>();
    }

    [Fact]
    public async Task ListBookingsByMember_ReturnCorrectBookings()
    {
        var gym2 = await CreateGym("Gym Two");
        var room2 = await CreateRoom(gym2.Id, "Room Two");
        var memberPt = await CreateMember(gym2.Id, "Pt-Member");
        var trainer2 = await CreateTrainer(memberPt.Id, "PT2", "923000033", "PT22@gmail.com");

        var member2 = await CreateMember(gym2.Id, "Member 2");
        var subscription2 = await CreateSubscription(SubscriptionType.Basic, member2.Id);
        var session2 = await CreateSession(room2.Id, trainer2.Id, "Session 2");
        await CreateBooking(session2.Id, member2.Id, room2.Id, subscription2.Id);


        var member3 = await CreateMember(_gym.Id, "Member 3");
        var subscription3 = await CreateSubscription(SubscriptionType.Plus, member3.Id);
        var bookingMember3 = await CreateBooking(_session.Id, member3.Id, _room.Id, subscription3.Id);

        var cancelBookingResult = await _mediator.Send(new CancelBookingCommand(Id: bookingMember3.Id));
        cancelBookingResult.IsError.Should().BeFalse();

        // await CreateBooking(_session.Id, member3.Id, _room.Id, subscription3.Id);
        var createResult = await _mediator.Send(new CreateBookingCommand(SessionId: _session.Id, MemberId: member3.Id));
        createResult.IsError.Should().BeFalse();

        var listResult1 = await _mediator.Send(new ListBookingsByMemberQuery(MemberId: _participant.Id));
        listResult1.IsError.Should().BeFalse();
        var bookings1 = listResult1.Value.ToList();

        var listResult2 = await _mediator.Send(new ListBookingsByMemberQuery(MemberId: member3.Id));
        listResult2.IsError.Should().BeFalse();
        var bookings2 = listResult2.Value.ToList();

        // Assert        
        bookings1.Count().Should().Be(1);
        bookings1.Should().BeOfType<List<BookingDto>>();
        bookings2.Count().Should().Be(2);
        bookings2.Should().BeOfType<List<BookingDto>>();
    }
    
    [Fact]
    public async Task ListBookingsBySession_ReturnCorrectBookings()
    {        
        var member2 = await CreateMember(_gym.Id, "Member 2");
        var subscription2 = await CreateSubscription(SubscriptionType.Basic, member2.Id);
        // var session2 = await CreateSession(room2.Id, trainer2.Id, "Session 2");
        await CreateBooking(_session.Id, member2.Id, _room.Id, subscription2.Id);
   
        var listResult1 = await _mediator.Send(new ListBookingsBySessionQuery(SessionId: _session.Id));
        listResult1.IsError.Should().BeFalse();
        var bookings1 = listResult1.Value.ToList();

        // Assert        
        bookings1.Count().Should().Be(2);
        bookings1.Should().BeOfType<List<BookingDto>>();
    }
}