using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Sessions.Events;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Sessions;

public class SessionTests
{
    [Fact]
    public void CreateSession_ShouldSetStatusToScheduled()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);

        // Assert
        session.Status.Should().Be(SessionStatus.Scheduled);
    }

     [Fact]
    public void CreateSession_SetCapacityCorrectly()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);

        // Assert
        session.Capacity.Should().Be(Constants.Rooms.Capacity);
        session.Vacancy.Should().Be(Constants.Rooms.Capacity);
    }

    [Fact]
    public void Cancel_ActiveSession_ChangeStatusToCanceled()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                 trainerId: Constants.Trainers.Id,
                                 title: Constants.Sessions.Title,
                                 capacity: Constants.Rooms.Capacity,
                                 vacancy: Constants.Rooms.Capacity,
                                 startDate: Constants.Sessions.StartDate,
                                 endDate: Constants.Sessions.EndDate);

        // Act 
        var result = session.Cancel();

        // Assert
        result.IsError.Should().BeFalse();
        session.Status.Should().Be(SessionStatus.Canceled);
    }

    // TODO: Test Session cancelection side effect all active bookings are Cancelled
    [Fact]
    public void Cancel_ShouldRaiseSessionCanceledDomainEvent()
    {
        // Arrange        
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);

        // Act
        var canceledResult = session.Cancel();
           
        var domainEvent = session.PopAndClearDomainEvents()
                                    .OfType<SessionCanceledEvent>()
                                    .SingleOrDefault();
        // Assert 
        canceledResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.SessionId.Should().Be(session.Id);
    }
    
    [Fact]
    public void Cancel_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);
        session.Cancel();

        // Act 
        var result = session.Cancel();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(session.Id));
    }

    [Fact]
    public void Finalize_ActiveSession_ChangeStatusToFinalized()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);

        // Act 
        var result = session.Finalize();

        // Assert
        result.IsError.Should().BeFalse();
        session.Status.Should().Be(SessionStatus.Finalized);
    }
    
     [Fact]
    public void Finalize_ShouldRaiseSessionFinalizedDomainEvent()
    {
        // Arrange        
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);

        // Act
        var finalized = session.Finalize();
           
        var domainEvent = session.PopAndClearDomainEvents()
                                    .OfType<SessionFinalizedEvent>()
                                    .SingleOrDefault();
        // Assert 
        finalized.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.SessionId.Should().Be(session.Id);
    }
    
    [Fact]
    public void Finalize_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.NewId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);
        session.Finalize();

        // Act 
        var result = session.Finalize();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(session.Id));
    }
    
}