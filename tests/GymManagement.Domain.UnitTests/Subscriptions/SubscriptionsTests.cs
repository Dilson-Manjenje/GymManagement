using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Subscriptions.Events;
using TestCommon.Rooms;
using TestCommon.Subscriptions;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Subscriptions;

public class SubscriptionsTests
{

    [Fact]
    public void Create_ShouldCreateActiveSubscription()
    {
        var subscription = new Subscription(subscriptionType: Constants.Subscriptions.DefaultSubscriptionType,
                                            memberId: Constants.Members.NewId);

        // Assert
        subscription.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_Subscription_ShouldSetCorrectEndDate()
    {
        // Arrange
        var subscription = new Subscription(subscriptionType: Constants.Subscriptions.DefaultSubscriptionType,
                                            memberId: Constants.Members.NewId);

        // Assert
        subscription.EndDate.Should().Be(subscription.StartDate.AddDays(subscription.SubscriptionType.DurationInDays));
    }

    [Fact]
    public void Disable_ActiveSubscription_ChangeIsActiveToFalse()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var disableResult = subscription.Disable();

        // Assert 
        disableResult.IsError.Should().BeFalse();
        subscription.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Disable_ActiveSubscription_ShouldRaiseSubscriptionDisabledDomainEvent()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var disableResult = subscription.Disable();
        var domainEvent = subscription.PopAndClearDomainEvents()
                                    .OfType<SubscriptionDisabledEvent>()
                                    .SingleOrDefault();
        // Assert 
        disableResult.IsError.Should().BeFalse();
        domainEvent.Should().NotBeNull();
        domainEvent?.SubscriptionId.Should().Be(subscription.Id);
    }

    [Fact]
    public void Disable_InactiveSubscription_ReturnCantChangeExpiredSubscriptionError()
    {        
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        subscription.Disable();
     
        var disableResult = subscription.Disable();

        // Assert 
        disableResult.IsError.Should().BeTrue();
        disableResult.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());
        subscription.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Update_ActiveSubscription_ChangeSubscriptionType()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        var result = subscription.Update(Constants.Subscriptions.DefaultSubscriptionType);

        // Assert 
        result.IsError.Should().BeFalse();
        subscription.SubscriptionType.Should().Be(SubscriptionType.Basic);
    }

     [Fact]
    public void Update_InactiveSubscription_ReturnCantChangeExpiredSubscriptionError()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        subscription.Disable();

        var result = subscription.Update(Constants.Subscriptions.DefaultSubscriptionType);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());
        subscription.IsActive.Should().BeFalse();
    }


    [Fact]
    public void AddRoom_AddTheRoomToSubscriptionRooms()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);

        var fightRoom = RoomFactory.GetKickBoxigRoomOfFightGym();
        var swimmingRoom = RoomFactory.CreateRoom("Swimming Room");

        var result = subscription.AddRoom(fightRoom.Id);
        var secondResult = subscription.AddRoom(swimmingRoom.Id);

        // Assert 
        result.IsError.Should().BeFalse();
        secondResult.IsError.Should().BeFalse();
        subscription.NumberOfRooms.Should().Be(2);
    }

    [Fact]
    public void AddRoom_MoreThanSubscriptionAllows_ReturnHasMaxRoomsError()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                  memberId: Constants.Members.FightStudent1);

        var rooms = Enumerable.Range(0, subscription.MaxRoomsAllowed + 1)
                    .Select(_ => RoomFactory.CreateRoom())
                    .ToList();
        
        //var addRoomResult = rooms.ConvertAll(subscription.AddRoom); // Use when add room object instead of roomId
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
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Basic,
                                                                  memberId: Constants.Members.FightStudent1);

        var rooms = Enumerable.Range(0, subscription.MaxRoomsAllowed)
                    .Select(_ => RoomFactory.CreateRoom())
                    .ToList();
        
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
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Basic,
                                                                  memberId: Constants.Members.FightStudent1);
        var room = RoomFactory.CreateRoom();

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
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        var firstResult = subscription.AddRoom(defaultRoom.Id);
        subscription.Disable();
        var secondResult = subscription.AddRoom(swimmingRoom.Id);

        // Assert 
        firstResult.IsError.Should().BeFalse();
        secondResult.IsError.Should().BeTrue();
        secondResult.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());

    }

    [Fact]
    public void RemoveRoom_WhenRoomIsInSubscription_RemoveWithSucess()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        subscription.AddRoom(defaultRoom.Id);
        subscription.AddRoom(swimmingRoom.Id);
        var removeResult = subscription.RemoveRoom(swimmingRoom.Id);

        // Assert 
        removeResult.IsError.Should().BeFalse();
        subscription.NumberOfRooms.Should().Be(1);
    }

    [Fact]
    public void RemoveRoom_WhenRoomNotInSubscription_ReturnRoomNotAssociatedError()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        var defaultRoom = RoomFactory.CreateRoom();
        var swimmingRoom = RoomFactory.CreateRoom(name: "Swimming Room");

        subscription.AddRoom(defaultRoom.Id);
        var removeResult = subscription.RemoveRoom(swimmingRoom.Id);

        // Assert 
        removeResult.IsError.Should().BeTrue();
        removeResult.FirstError.Should().Be(SubscriptionErrors.RoomNotInSubscription(swimmingRoom.Id));
    }

    [Fact]
    public void RemoveRoom_WhenSubscriptionDisabled_ReturnCantChangeExpiredSubscriptionError()
    {
        var subscription = SubscriptionFactory.CreateSubscription(type: SubscriptionType.Plus,
                                                                  memberId: Constants.Members.FightStudent1);
        var defaultRoom = RoomFactory.CreateRoom();
        subscription.AddRoom(defaultRoom.Id);

        subscription.Disable();
        var result = subscription.RemoveRoom(defaultRoom.Id);

        // Assert 
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());
    }

}