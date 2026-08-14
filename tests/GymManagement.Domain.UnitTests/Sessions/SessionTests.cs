using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Sessions.Events;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Sessions;

public class SessionTests
{
    // TODO: Add Session.Create() to a SetUp method~
    private Session _session;

    // Runs BEFORE each test (NUnit [SetUp])
    public SessionTests()
    {
        _session = Session.Create(roomId: Constants.Rooms.KickBoxingRoomId,
                                  trainerId: Constants.Trainers.Id,
                                  title: Constants.Sessions.Title,
                                  capacity: Constants.Rooms.Capacity,
                                  vacancy: Constants.Rooms.Capacity,
                                  startDate: Constants.Sessions.StartDate,
                                  endDate: Constants.Sessions.EndDate);
    }

    // Runs AFTER each test (NUnit [TearDown])
    // public void Dispose(){}
    
    [Fact]
    public void CreateSession_ShouldSetStatusToScheduled()
    {
        // Arrange
        // Assert
        _session.Status.Should().Be(SessionStatus.Scheduled);
    }

     [Fact]
    public void CreateSession_SetCapacityCorrectly()
    {
       
        // Assert
        _session.Capacity.Should().Be(Constants.Rooms.Capacity);
        _session.Vacancy.Should().Be(Constants.Rooms.Capacity);
    }

    [Fact]
    public void Cancel_ActiveSession_ChangeStatusToCanceled()
    {
        // Arrange
     
        // Act 
        var result = _session.Cancel();

        // Assert
        result.IsError.Should().BeFalse();
        _session.Status.Should().Be(SessionStatus.Canceled);
    }

    // TODO: Test Session canceletion side effect all active bookings are Canceled
    [Fact]
    public void Cancel_ShouldRaiseSessionCanceledDomainEvent()
    {
        // Arrange        
       
        // Act
        var canceledResult = _session.Cancel();
           
        var domainEvent = _session.PopAndClearDomainEvents()
                                    .OfType<SessionCanceledEvent>()
                                    .SingleOrDefault();
        // Assert 
        canceledResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.SessionId.Should().Be(_session.Id);
    }
    
    [Fact]
    public void Cancel_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange
  
        _session.Cancel();

        // Act 
        var result = _session.Cancel();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(_session.Id));
    }

    [Fact]
    public void Finalize_ActiveSession_ChangeStatusToFinalized()
    {
        // Arrange

        // Act 
        var result = _session.Finalize();

        // Assert
        result.IsError.Should().BeFalse();
        _session.Status.Should().Be(SessionStatus.Finalized);
    }
    
     [Fact]
    public void Finalize_ShouldRaiseSessionFinalizedDomainEvent()
    {
        // Arrange        

        // Act
        var finalized = _session.Finalize();
           
        var domainEvent = _session.PopAndClearDomainEvents()
                                    .OfType<SessionFinalizedEvent>()
                                    .SingleOrDefault();
        // Assert 
        finalized.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.SessionId.Should().Be(_session.Id);
    }
    
    [Fact]
    public void Finalize_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange

        _session.Finalize();

        // Act 
        var result = _session.Finalize();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SessionErrors.CantChangeSession(_session.Id));
    }
    
}