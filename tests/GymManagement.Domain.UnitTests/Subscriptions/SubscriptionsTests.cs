using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Subscriptions;
using TestCommon.Rooms;
using TestCommon.Subscriptions;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Subscriptions;

public class SubscriptionsTests
{
    [Fact]
    public void AddRoom_MoreThanSubscriptionAllows_ReturnHasMaxRoomsError()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                  memberId: Constants.Members.FightStudent1);

        var rooms = Enumerable.Range(0, subscription.MaxRoomsAllowed + 1)
                    .Select(_ => RoomFactory.CreateRoom())
                    .ToList();
        // Act
        //var addRoomResult = rooms.ConvertAll(subscription.AddRoom); // When add Room object instead of RoomId
        List<ErrorOr<Success>> result = new();
        foreach (var room in rooms)
            result.Add(subscription.AddRoom(room.Id));

        var lastAddRoomResult = result.Last();

        // Assert 
        lastAddRoomResult.IsError.Should().BeTrue();
        lastAddRoomResult.FirstError.Should().Be(SubscriptionErrors.HasMaxRoomsAllowed());
    }

    [Fact]
    public void AddRoom_LessOrIgualThanSubscriptionAllows_AddWithSuccess()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Basic,
                                                                  memberId: Constants.Members.FightStudent1);

        var rooms = Enumerable.Range(0, subscription.MaxRoomsAllowed)
                    .Select(_ => RoomFactory.CreateRoom())
                    .ToList();
        // Act
        List<ErrorOr<Success>> result = new();
        foreach (var room in rooms)
            result.Add(subscription.AddRoom(room.Id));

        var lastAddRoomResult = result.Last();

        // Assert 
        lastAddRoomResult.IsError.Should().BeFalse();
    }

    [Fact]
    public void AddRoom_WhenRoomAlreadyExist_ReturnRoomAlreadyExistError()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Basic,
                                                                  memberId: Constants.Members.FightStudent1);

        var room = RoomFactory.CreateRoom();

        // Act
        var firstResult = subscription.AddRoom(room.Id);
        var secondResult = subscription.AddRoom(room.Id);

        // Assert 
        firstResult.IsError.Should().BeFalse();
        secondResult.IsError.Should().BeTrue();
        secondResult.FirstError.Should().Be(SubscriptionErrors.RoomAlreadyAssociated(room.Id));

    }

    [Fact]
    public void AddRoom_WhenSubscriptionDisabled_ReturnCantChangeExpiredSubscriptionError()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        // Act
        var firstResult = subscription.AddRoom(defaultRoom.Id);

        subscription.DisableSubscription();
        var secondResult = subscription.AddRoom(swimmingRoom.Id);

        // Assert 
        firstResult.IsError.Should().BeFalse();
        secondResult.IsError.Should().BeTrue();
        secondResult.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());

    }

    [Fact]
    public void RemoveRoom_WhenRoomNotInSubscription_ReturnRoomNotAssociatedError()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        // Act
        subscription.AddRoom(defaultRoom.Id);
        var removeResult = subscription.RemoveRoom(swimmingRoom.Id);

        // Assert 
        removeResult.IsError.Should().BeTrue();
        removeResult.FirstError.Should().Be(SubscriptionErrors.RoomNotInSubscription(swimmingRoom.Id));
    }


    [Fact]
    public void RemoveRoom_WhenRoomIsInSubscription_RemoveWithSucess()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        // Act
        subscription.AddRoom(defaultRoom.Id);
        subscription.AddRoom(swimmingRoom.Id);
        var removeResult = subscription.RemoveRoom(swimmingRoom.Id);

        // Assert 
        removeResult.IsError.Should().BeFalse();
        subscription.NumberOfRooms.Should().Be(1);
    }

    [Fact]
    public void DisableSubscription_ShouldDisabledWithSucess()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        // Act
        var disableResult = subscription.DisableSubscription();

        // Assert 
        disableResult.IsError.Should().BeFalse();
        subscription.IsActive.Should().BeFalse();
    }
    
    
    [Fact]
    public void UpdateSubscription_ShouldChangeSubscriptionTypedWithSucess()
    {
        // Arrange
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        // Act
        var result = subscription.UpdateSubscription(Constants.Subscriptions.DefaultSubscriptionType);

        // Assert 
        result.IsError.Should().BeFalse();
        subscription.SubscriptionType.Should().Be(SubscriptionType.Basic);        
    }
}