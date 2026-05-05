using FluentAssertions;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Bookings.Events;
using GymManagement.Domain.Members;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Subscriptions;
using TestCommon.Members;
using TestCommon.Rooms;
using TestCommon.Subscriptions;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.BookingTests;

public class BookingTests
{
    private Member _member;
    private Room _room;
    private Subscription _subscription;
    private Session _session;
    private Booking _booking;

    // Runs BEFORE each test (NUnit [SetUp])
    public BookingTests()
    {
        _member = MembersFactory.CreateMember(gymId: Constants.Gyms.FightGymId);
        _subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                               memberId: _member.Id);

        _room = RoomFactory.GetKickBoxigRoomOfFightGym();
        _subscription.AddRoom(_room.Id);

        _session = Session.Create(roomId: Constants.Rooms.KickBoxingRoomId,
                                     trainerId: Constants.Trainers.Id,
                                     title: Constants.Sessions.Title,
                                     capacity: Constants.Rooms.Capacity,
                                     vacancy: Constants.Rooms.Capacity,
                                     startDate: Constants.Sessions.StartDate,
                                     endDate: Constants.Sessions.EndDate);
        _session.Room = _room;
        
        _booking = Booking.Create(_member, _session, _subscription).Value;
    }

    // Runs AFTER each test (NUnit [TearDown])
    // public void Dispose(){}

    [Fact]
    public void Create_WhenSubscriptionHaveTheRoom_CreateActiveBooking()
    {      
        var bookingResult = Booking.Create(_member, _session, _subscription);


        bookingResult.IsError.Should().BeFalse();
        bookingResult.Value.Status.Should().Be(BookingStatus.Active);

    }

    [Fact]
    public void Create_WhenSubscriptionDontHaveAccessToTheRoom_ReturnSubscriptionDontHaveAccessError()
    {
        // arrange

        _subscription.RemoveRoom(_room.Id);

        // act        
        var bookingResult = Booking.Create(_member, _session, _subscription);

        // Assert
        bookingResult.IsError.Should().BeTrue();
        bookingResult.FirstError.Should().Be(BookingErrors.SubscriptionDontHaveAccess(subscriptionId: _subscription.Id, roomId: _session.RoomId));
    }

    [Fact]
    public void Create_ShouldRaiseBookingCreatedDomainEvent()
    {
        var createResult = Booking.Create(_member, _session, _subscription);
        var booking = createResult.Value;

        var domainEvent = booking.PopAndClearDomainEvents()
                                 .OfType<BookingCreatedEvent>()
                                 .SingleOrDefault();


        createResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.BookingId.Should().Be(booking.Id);
    }

    [Fact]
    public void Create_WithinSessionCapacity_SetVacancyCorrectly()
    {
        // Act
        var result1 = Booking.Create(_member, _session, _subscription);
        _session.DecrementVacancy();
        var result2 = Booking.Create(_member, _session, _subscription);
        _session.DecrementVacancy();

        // Assert
        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();
        _session.Vacancy.Should().Be(0);
    }

    [Fact]
    public void Create_WhenSessionIsFull_ReturnCannotExceedSessionCapacityError()
    {

        // Arrange
        var result1 = Booking.Create(_member, _session, _subscription);
        _session.DecrementVacancy();
        var result2 = Booking.Create(_member, _session, _subscription);
        _session.DecrementVacancy();

        // act        
        var bookingResult = Booking.Create(_member, _session, _subscription);

        // Assert
        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();
        bookingResult.IsError.Should().BeTrue();
        bookingResult.FirstError.Should().Be(SessionErrors.CannotExceedSessionCapacity);
    }

    [Fact]
    public void Create_WhenMemberHaveDifferentGym_ReturnMemberNotInTheSameGymError()
    {

        // Arrange
        _member = MembersFactory.CreateMember(gymId: Constants.Gyms.NewId);

        // act        
        var bookingResult = Booking.Create(_member, _session, _subscription);

        // Assert
        bookingResult.IsError.Should().BeTrue();
        bookingResult.FirstError.Should().Be(BookingErrors.MemberNotInTheSameGym(_member.Id));
    }

    [Fact]
    public void Create_WhenSessionIsCanceled_ReturnInvalidSessionsStatusError()
    {

        // Arrange
        _session.Cancel();

        // act        
        var bookingResult = Booking.Create(_member, _session, _subscription);

        // Assert
        bookingResult.IsError.Should().BeTrue();
        bookingResult.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(_session.Id, statusName: _session.Status.Name));
    }

    [Fact]
    public void Create_WhenSessionIsdFinalized_ReturnInvalidSessionsStatusError()
    {

        // Arrange
        _session.Finalize();

        // act        
        var bookingResult = Booking.Create(_member, _session, _subscription);

        // Assert
        bookingResult.IsError.Should().BeTrue();
        bookingResult.FirstError.Should().Be(BookingErrors.InvalidSessionsStatus(_session.Id, statusName: _session.Status.Name));
    }

    [Fact]
    public void Cancel_ActiveBooking_ChangeStatusToCanceled()
    {
        var canceledResult = _booking.Cancel();

        // assert
        canceledResult.IsError.Should().BeFalse();
        _booking.Status.Should().Be(BookingStatus.Canceled);
    }

    [Fact]
    public void Cancel_ActiveBooking_ShouldRaiseBookingCanceledEvent()
    {
        var canceledResult = _booking.Cancel();

        var domainEvent = _booking.PopAndClearDomainEvents()
                                 .OfType<BookingCanceledEvent>()
                                 .SingleOrDefault();
        // Assert 
        canceledResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.BookingId.Should().Be(_booking.Id);
    }

    [Fact]
    public void Cancel_WhenBookingIsCanceled_ReturnCantChangeBookingError()
    {
        var result1 = _booking.Cancel();
        var canceledResult = _booking.Cancel();

        // assert
        result1.IsError.Should().BeFalse();
        canceledResult.IsError.Should().BeTrue();
        canceledResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(_booking.Id, _booking.Status.Name));
    }

    [Fact]
    public void Cancel_WhenBookingIsFinalized_ReturnCantChangeBookingError()
    {
        var result1 = _booking.Finalize();

        var canceledResult = _booking.Cancel();

        result1.IsError.Should().BeFalse();
        canceledResult.IsError.Should().BeTrue();
        canceledResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(_booking.Id, _booking.Status.Name));
    }

    [Fact]
    public void Finalize_ActiveBooking_ChangeStatusToFinalized()
    {
        var result = _booking.Finalize();

        // assert
        result.IsError.Should().BeFalse();
        _booking.Status.Should().Be(BookingStatus.Finalized);
    }

    [Fact]
    public void Finalize_ActiveBooking_ShouldRaiseBookingFinalizedEvent()
    {
        var canceledResult = _booking.Finalize();

        var domainEvent = _booking.PopAndClearDomainEvents()
                                 .OfType<BookingFinalizedEvent>()
                                 .SingleOrDefault();
        // Assert 
        canceledResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.BookingId.Should().Be(_booking.Id);
    }

    [Fact]
    public void Finalize_WhenBookingIsCanceled_ReturnCantChangeBookingError()
    {
        var result1 = _booking.Cancel();
        var canceledResult = _booking.Finalize();

        result1.IsError.Should().BeFalse();
        canceledResult.IsError.Should().BeTrue();
        canceledResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(_booking.Id, _booking.Status.Name));
    }
    
     [Fact]
    public void Finalize_WhenBookingIsFinalized_ReturnCantChangeBookingError()
    {
        var result1 = _booking.Finalize();

        var canceledResult = _booking.Finalize();

        result1.IsError.Should().BeFalse();
        canceledResult.IsError.Should().BeTrue(); 
        canceledResult.FirstError.Should().Be(BookingErrors.CantChangeBooking(_booking.Id, _booking.Status.Name));        
    }
}