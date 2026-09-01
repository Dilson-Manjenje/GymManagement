using MediatR;
using TestCommon.Subscriptions;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using ErrorOr;
using TestCommon.Gyms;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Members;
using FluentAssertions.Equivalency;
using TestCommon.Members;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using TestCommon.Rooms;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;
using GymManagement.Application.Subscriptions.Commands.DisableSubscription;
using GymManagement.Application.Subscriptions.Commands.UpdateSubscription;
using Microsoft.EntityFrameworkCore.Metadata;
using GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

namespace GymManagement.Application.SubcutaneousTests.Subscriptions;

[Collection(MediatorFactoryCollection.CollectionName)]
public class SubscriptionAppTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateSubscription_WhenCommandIsValid_ShouldReturnId()
    {
        // Arrange 
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);

        // Act
        var createSubsCommand = SubscriptionCommandFactory.CreateCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: member.Id);

        var result = await _mediator.Send(createSubsCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    private async Task<GymDto> CreateGym()
    {
        var createGymCommand = GymCommandFactory.CreateGymCommand(Constants.Gyms.Name, Constants.Gyms.Address);
        var createGymResult = await _mediator.Send(createGymCommand);

        var queryResult = await _mediator.Send(new GetGymQuery(GymId: createGymResult.Value));

        // Assert
        createGymResult.IsError.Should().BeFalse();
        createGymResult.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    private async Task<MemberDto> CreateMember(Guid gymId)
    {
        var createMemberCommand = MemberCommandFactory.CreateMemberCommand(gymId: gymId,
                                                                            userName: "member1",
                                                                            password: "Abc123");
        var createMemberResult = await _mediator.Send(createMemberCommand);

        var queryResult = await _mediator.Send(new GetMemberQuery(MemberId: createMemberResult.Value));


        // Assert
        createMemberResult.IsError.Should().BeFalse();
        createMemberResult.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }

    [Fact]
    public async Task CreateSubscription_WhenMemberDontExist_ShouldReturnMemberNotFoundError()
    {
        // Arrange 
        var memberId = Guid.NewGuid();

        // Act
        var createSubsCommand = SubscriptionCommandFactory.CreateCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: memberId);

        var result = await _mediator.Send(createSubsCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound(memberId));

    }

    [Fact]
    public async Task CreateSubscription_WhenMemberHasActiveSubscription_ReturnMemberHaveActiveSubscriptionError()
    {
        // Arrange 
        var gym = await CreateGym();
        var member = await CreateMember(gymId: gym.Id);

        // Act
        var command1 = SubscriptionCommandFactory.CreateCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: member.Id);

        var result1 = await _mediator.Send(command1);
        result1.IsError.Should().BeFalse();

        var command2 = SubscriptionCommandFactory.CreateCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: member.Id);
        var result2 = await _mediator.Send(command2);

        // Assert
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(MemberErrors.MemberAlreadyHaveActiveSubscription(member.Id));

    }

    [Fact]
    public async Task Disable_ActiveSubscription_ShouldChangeSubscriptionStatus()
    {
        // Arrange
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);

        var subscription = await CreateSubscription(type: SubscriptionType.Plus, memberId: member.Id);

        // Act
        var disableSubsCmd = new DisableSubscriptionCommand(SubscriptionId: subscription.Id);

        var disableSubsResult = await _mediator.Send(disableSubsCmd);
        disableSubsResult.IsError.Should().BeFalse();

        var getSubsResult = await _mediator.Send(new GetSubscriptionQuery(Id: subscription.Id));
        getSubsResult.IsError.Should().BeFalse();
        getSubsResult.Value.Should().NotBeNull();

        subscription = getSubsResult.Value;

        // Assert                 
        subscription.IsActive.Should().BeFalse();
        // subscription.EndDate.Should().BeBefore(DateTime.UtcNow);

    }

    [Fact]
    public async Task Disable_WhenSubscriptionDontExist_ShouldReturnNoFoundError()
    {
        // Act
        var disableSubsCmd = new DisableSubscriptionCommand(SubscriptionId: Guid.NewGuid());

        var disableSubsResult = await _mediator.Send(disableSubsCmd);

        // Assert                 
        disableSubsResult.IsError.Should().BeTrue();
        disableSubsResult.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound(disableSubsCmd.SubscriptionId));
    }

    [Fact]
    public async Task Disable_ExpiredSubscription_ShouldReturnCantChangeExpiredSubscriptionError()
    {
        // Act
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var disableCmd = new DisableSubscriptionCommand(subscription.Id);
        var result1 = await _mediator.Send(disableCmd);
        result1.IsError.Should().BeFalse();

        var disableCmd2 = new DisableSubscriptionCommand(subscription.Id);
        var result2 = await _mediator.Send(disableCmd2);

        // Assert                 
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());

    }
    
    private async Task<SubscriptionDto> CreateSubscription(SubscriptionType type, Guid memberId)
    {
        var command = SubscriptionCommandFactory.CreateCreateSubscriptionCommand(type: type, memberId: memberId);

        var result = await _mediator.Send(command);

        // Ensure create sucessfully
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSubscriptionQuery(Id: result.Value));
        result.IsError.Should().BeFalse();

        return queryResult.Value;
    }

    [Fact]
    public async Task Disable_ActiveSubscription_ShouldRemoveAllSubscriptionRooms()
    {
        // Arrange
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room1 = await CreateRoom(gym.Id, "Swimming Room", 2);
        var room2 = await CreateRoom(gym.Id, "Fight Room", 2);

        var subscription = await CreateSubscription(type: SubscriptionType.Plus, memberId: member.Id);
        var addRoom1Cmd = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id,
                                                           RoomId: room1.Id);

        var addRoom1Result = await _mediator.Send(addRoom1Cmd);
        addRoom1Result.IsError.Should().BeFalse();

        var addRoom2Cmd = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id,
                                                              RoomId: room2.Id);
        var addRoom2Result = await _mediator.Send(addRoom2Cmd);
        addRoom2Result.IsError.Should().BeFalse();

        // Act
        var disableSubsCmd = new DisableSubscriptionCommand(SubscriptionId: subscription.Id);

        var disableSubsResult = await _mediator.Send(disableSubsCmd);
        disableSubsResult.IsError.Should().BeFalse();

        var getSubsResult = await _mediator.Send(new GetSubscriptionQuery(Id: subscription.Id));

        // Assert                 
        getSubsResult.IsError.Should().BeFalse();
        getSubsResult.Value.Should().NotBeNull();
        getSubsResult.Value.Rooms?.Count().Should().Be(0);

    }
    
     private async Task<RoomDto> CreateRoom(Guid gymId, string? name = null, int capacity = 0)
    {
        var command = RoomCommandFactory.CreateRoomCommand(gymId: gymId,
                                                           name: name, capacity: capacity);

        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        var queryResult = await _mediator.Send(new GetRoomQuery(RoomId: result.Value));
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        return queryResult.Value;
    }


    [Fact]
    public async Task Update_SubscriptionWhenCommandIsValid_UpdateSubscriptionType()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(Constants.Subscriptions.DefaultSubscriptionType, member.Id);

        // Act 
        var cmd = new UpdateSubscriptionCommand(subscription.Id, SubscriptionType.Plus);
        var result = await _mediator.Send(cmd);
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSubscriptionQuery(Id: result.Value));
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        subscription = queryResult.Value;
        
        // assert 
        subscription.SubscriptionType.Should().Be(SubscriptionType.Plus);
        
    }
    
    [Fact]
    public async Task Update_WhenSubscriptionDontExist_ShouldReturnNoFoundError()
    {
        // Act
        var command = new UpdateSubscriptionCommand(Id: Guid.NewGuid(), SubscriptionType: SubscriptionType.Basic);

        var result = await _mediator.Send(command);

        // Assert                 
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound(command.Id));
    }

    [Fact]
    public async Task Update_WhenSubscriptionIsExpired_ShouldReturnCantChangeExpiredSubscriptionError()
    {
        // Act
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);
        
        var disableCmd = new DisableSubscriptionCommand(subscription.Id);
        var result1 = await _mediator.Send(disableCmd);
        result1.IsError.Should().BeFalse();

        var updateCmd = new UpdateSubscriptionCommand(Id: subscription.Id, SubscriptionType: SubscriptionType.Basic);

        var result2 = await _mediator.Send(updateCmd);

        // Assert                 
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());
    }


    [Fact]
    public async Task Delete_ActiveSubscription_ReturnCantDeleteActiveSubscriptionError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(Constants.Subscriptions.DefaultSubscriptionType, member.Id);

        var cmd = new DeleteSubscriptionCommand(subscription.Id);
        var result = await _mediator.Send(cmd);

        // assert         
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.CantDeleteActiveSubscription());
    }

    [Fact]
    public async Task Delete_ExpiredSubscription_DeleteSubscription()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(Constants.Subscriptions.DefaultSubscriptionType, member.Id);

        var disableCmd = new DisableSubscriptionCommand(subscription.Id);
        var disableResult = await _mediator.Send(disableCmd);
        disableResult.IsError.Should().BeFalse();

        // Act 
        var deleteCmd = new DeleteSubscriptionCommand(subscription.Id);
        var deleteResult = await _mediator.Send(deleteCmd);
        deleteResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSubscriptionQuery(Id: subscription.Id));

        // assert 
        queryResult.IsError.Should().BeTrue();
        queryResult.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound(subscription.Id));
    }

    [Fact]
    public async Task Delete_WhenSubscriptionDontExist_ShouldReturnNoFoundError()
    {
        // Act
        var command = new DeleteSubscriptionCommand(SubscriptionId: Guid.NewGuid());

        var result = await _mediator.Send(command);

        // Assert                 
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId));
    }
        
}