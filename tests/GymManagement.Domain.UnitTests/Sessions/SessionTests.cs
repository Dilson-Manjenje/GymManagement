using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Sessions;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Sessions;

public class SessionTests
{
    [Fact]
    public void CreateSession_ShouldSetStatusToScheduled()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.Id,
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
    public void Cancel_ActiveSession_ChangeStatusToCanceled()
    {
        // Arrange
         var session = Session.Create(roomId: Constants.Rooms.Id,
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

    [Fact]
    public void Cancel_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.Id,
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
        var session = Session.Create(roomId: Constants.Rooms.Id,
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
    public void Finalize_SessionInNonCancelableStatus_ReturnCantChangeSessionError()
    {
        // Arrange
        var session = Session.Create(roomId: Constants.Rooms.Id,
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