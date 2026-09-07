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
using GymManagement.Application.Trainers.Queries.Dtos;
using TestCommon.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Application.Sessions.Commands.CreateSession;
using GymManagement.Application.Sessions.Queries.GetSession;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Bookings.Commands.CreateBooking;
using GymManagement.Application.Bookings.Queries.GetBooking;
using GymManagement.Domain.Rooms;
using GymManagement.Application.Subscriptions.Commands.RemoveRoomFromSubscription;

namespace GymManagement.Application.SubcutaneousTests.Subscriptions;

[Collection(MediatorFactoryCollection.CollectionName)]
public class SubscriptionAppTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateSubscription_WhenCommandIsValid_CreateActiveSubscription()
    {
        // Arrange 
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);

        // Act
        var createCmd = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: member.Id);
        var createResult = await _mediator.Send(createCmd);
        
        createResult.IsError.Should().BeFalse();
        createResult.Value.Should().NotBeEmpty();

        var query = new GetSubscriptionQuery(Id: createResult.Value);
        var queryResult = await _mediator.Send(query);
        var subscription = queryResult.Value;
        
        // Assert
        queryResult.IsError.Should().BeFalse();
        subscription.IsActive.Should().BeTrue();
        subscription.EndDate.Should().Be(subscription.StartDate.AddDays(subscription.SubscriptionType.DurationInDays));        
    }


    [Fact(Skip = "Not implemented", DisplayName = "ExpireSubscription_AfterEndDate")]
    public async Task Subscription_ExpireAfterEndDate()
    {
        // Arrange 
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        // Act
        Thread.Sleep(TimeSpan.FromMinutes(2));

        // Assert
        subscription.IsActive.Should().BeTrue();        
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

    [Fact]
    public async Task CreateSubscription_WhenMemberDontExist_ShouldReturnMemberNotFoundError()
    {
        // Arrange 
        var memberId = Guid.NewGuid();

        // Act
        var createSubsCommand = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
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
        var command1 = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
                                                                                           memberId: member.Id);

        var result1 = await _mediator.Send(command1);
        result1.IsError.Should().BeFalse();

        var command2 = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: Constants.Subscriptions.DefaultSubscriptionType,
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
        var command = SubscriptionCommandFactory.GetCreateSubscriptionCommand(type: type, memberId: memberId);

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

    [Fact]
    public async Task Delete_SubscriptionWithBookings_ReturnCantDeleteSubscriptionWithBookingsError()
    {
        // Assert
        var gym = await CreateGym();
        var room = await CreateRoom(gym.Id, "Yoga Room");
        var memberTrainer = await CreateMember(gym.Id, "pt1User");
        var trainer = await CreateTrainer(memberTrainer.Id);

        var memberParticipante = await CreateMember(gym.Id, "participante1");
        var subscription = await CreateSubscription(SubscriptionType.Plus, memberParticipante.Id);
        var addRoomCmd = new AddRoomToSubscriptionCommand(subscription.Id, room.Id);
        var addRoomResult = await _mediator.Send(addRoomCmd);
        addRoomResult.IsError.Should().BeFalse();

        var session = await CreateSession(room.Id, trainer.Id, "Session 1");
        var booking = await CreateBooking(session.Id, memberParticipante.Id);
        
        var disableSubsCmd = new DisableSubscriptionCommand(SubscriptionId: subscription.Id);
        var disableSubsResult = await _mediator.Send(disableSubsCmd);
        disableSubsResult.IsError.Should().BeFalse();

        // Act
        var command = new DeleteSubscriptionCommand(SubscriptionId: subscription.Id);

        var result = await _mediator.Send(command);

        // Assert                 
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.CantDeleteSubscriptionWithBookings(command.SubscriptionId));
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
    public async Task AddRoom_ActiveSubscription_ShouldAddRoomToListOfRooms()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room1 = await CreateRoom(gym.Id, "Swimming Room", 2);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room1.Id);

        var addRoom1Result = await _mediator.Send(command);
        addRoom1Result.IsError.Should().BeFalse();

        var getSubsResult = await _mediator.Send(new GetSubscriptionQuery(Id: subscription.Id));

        // Assert             
        getSubsResult.IsError.Should().BeFalse();
        getSubsResult.Value.Should().NotBeNull();
        getSubsResult.Value?.Rooms?.Count().Should().Be(1);

    }

    [Fact]
    public async Task AddRoom_WhenSubscriptionDontExist_ReturnNotFoundError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room1 = await CreateRoom(gym.Id, "Swimming Room", 2);
        Guid subsId = Guid.NewGuid();
        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subsId, RoomId: room1.Id);

        var result = await _mediator.Send(command);

        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound( subsId));

    }

    [Fact]
    public async Task AddRoom_WhenRoomDontExist_ReturnNotFoundError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: Guid.NewGuid());

        var result = await _mediator.Send(command);

        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.RoomId));
    }

    [Fact]
    public async Task AddRoom_WhenRoomAndMemberHasDifferentGym_ReturnRoomWasNotFoundInMemberGymError()
    {
        var gym1 = await CreateGym("Gym-One");
        var gym2 = await CreateGym("Gym-Two");
        var room = await CreateRoom(gym1.Id, "Swimming", 2);
        var member = await CreateMember(gym2.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room.Id);

        var result = await _mediator.Send(command);

        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.RoomWasNotFoundInMemberGym(roomId: room.Id));
    }

    [Fact]
    public async Task AddRoom_WhenRoomAlreadyExistInSubscription_ReturnRoomAlreadyAssociatedError()
    {
        var gym = await CreateGym("Gym-One");
        var room = await CreateRoom(gym.Id, "Swimming", 2);
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Plus, member.Id);

        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room.Id);
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var command2 = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room.Id);
        var result2 = await _mediator.Send(command2);

        // Assert             
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.RoomAlreadyAssociated(roomId: room.Id));
    }

    [Fact]
    public async Task AddRoom_MoreRoomsThanSubscriptionAllow_ReturnHasMaxRoomsAllowedError()
    {
        var gym = await CreateGym("Gym-One");
        var room1 = await CreateRoom(gym.Id, "Swimming", 2);
        var room2 = await CreateRoom(gym.Id, "fight", 2);
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var command = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room1.Id);
        var result1 = await _mediator.Send(command);
        result1.IsError.Should().BeFalse();

        var command2 = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room2.Id);
        var result2 = await _mediator.Send(command2);

        // Assert             
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.HasMaxRoomsAllowed());
    }

    [Fact]
    public async Task AddRoom_WhenSubscriptionIsInactive_ReturnCantChangeExpiredSubscriptionError()
    {
        var gym = await CreateGym("Gym-One");
        var room = await CreateRoom(gym.Id, "Swimming", 2);
        var member = await CreateMember(gym.Id);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var disableCmd = new DisableSubscriptionCommand(subscription.Id);
        var disableResult = await _mediator.Send(disableCmd);
        disableResult.IsError.Should().BeFalse();        

        var addCmd = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room.Id);
        var result = await _mediator.Send(addCmd);
        
        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());
    }

    [Fact]
    public async Task RemoveRoom_FromSubscription_RemoveRoomOfListRooms()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room1 = await CreateRoom(gym.Id, "Room 1", 2);
        var room2 = await CreateRoom(gym.Id, "Room 2", 2);
        var subscription = await CreateSubscription(SubscriptionType.Plus, member.Id);

        var addCmd1 = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room1.Id);
        var result1 = await _mediator.Send(addCmd1);
        result1.IsError.Should().BeFalse();

        var addCmd2 = new AddRoomToSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room2.Id);
        var result2 = await _mediator.Send(addCmd2);
        result2.IsError.Should().BeFalse();

        var removeCmd = new RemoveRoomFromSubscriptionCommand(SubscriptionId: subscription.Id, RoomId: room2.Id);
        var result3 = await _mediator.Send(removeCmd);
        result3.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetSubscriptionQuery(Id: subscription.Id));

        // Assert             
        queryResult.IsError.Should().BeFalse();
        queryResult.Value.Should().NotBeNull();
        queryResult.Value?.Rooms?.Count().Should().Be(1);

    }

    [Fact]
    public async Task RemoveRoom_WhenSubscriptionDontExist_ReturnNotFoundError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room = await CreateRoom(gym.Id, "Swimming", 2);
        var subsId = Guid.NewGuid();
        
        var command = new RemoveRoomFromSubscriptionCommand(subsId, room.Id);
        var result = await _mediator.Send(command);

        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(SubscriptionErrors.SubscriptionNotFound(subsId));

    }

    [Fact]
    public async Task RemoveRoom_WhenRoomDontExist_ReturnNotFoundError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room = await CreateRoom(gym.Id, "Swimming", 2);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var command = new RemoveRoomFromSubscriptionCommand(subscription.Id, Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert             
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoomErrors.RoomNotFound(command.RoomId));
    }

    [Fact]
    public async Task RemoveRoom_WhenRoomNotInSubscription_ReturnRoomNotInSubscriptionError()
    {
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room1 = await CreateRoom(gym.Id, "Room 1", 2);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);
        var room2 = await CreateRoom(gym.Id, "Room 2", 2);

        var addCmd = new AddRoomToSubscriptionCommand(subscription.Id, room1.Id);
        var result1 = await _mediator.Send(addCmd);
        result1.IsError.Should().BeFalse();

        var command = new RemoveRoomFromSubscriptionCommand(subscription.Id, room2.Id);
        var result2 = await _mediator.Send(command);

        // Assert             
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.RoomNotInSubscription(command.RoomId));
    }
    
     [Fact]
    public async Task RemoveRoom_WhenSubscriptionIsExpired_ShouldReturnCantChangeExpiredSubscriptionError()
    {
        // Act
        var gym = await CreateGym();
        var member = await CreateMember(gym.Id);
        var room = await CreateRoom(gym.Id, "Swimming", 2);
        var subscription = await CreateSubscription(SubscriptionType.Basic, member.Id);

        var disableCmd = new DisableSubscriptionCommand(subscription.Id);
        var result1 = await _mediator.Send(disableCmd);
        result1.IsError.Should().BeFalse();

        var removeCmd = new RemoveRoomFromSubscriptionCommand(subscription.Id, room.Id);
        var result2 = await _mediator.Send(removeCmd);

        // Assert                 
        result2.IsError.Should().BeTrue();
        result2.FirstError.Should().Be(SubscriptionErrors.CantChangeExpiredSubscription());

    }
    
}