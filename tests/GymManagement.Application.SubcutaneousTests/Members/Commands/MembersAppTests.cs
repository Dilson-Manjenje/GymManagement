using GymManagement.Application.SubcutaneousTests.Common;
using MediatR;
using TestCommon.Gyms;
using FluentAssertions;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Members;
using TestCommon.Members;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Application.Members.Commands.UpdateMember;
using GymManagement.Application.Members.Commands.DeleteMember;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using TestCommon.Subscriptions;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Application.Rooms.Queries.Dtos;
using TestCommon.Rooms;
using GymManagement.Application.Trainers.Queries.Dtos;
using TestCommon.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;
using GymManagement.Application.Bookings.Commands.CreateBooking;
using GymManagement.Application.Bookings.Queries.GetBooking;
using GymManagement.Application.Sessions.Commands.CreateSession;
using GymManagement.Application.Sessions.Queries.GetSession;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Members.Queries.ListMembers;
using GymManagement.Application.Members.Queries.ListMembersByGym;

namespace GymManagement.Application.SubcutaneousTests.Members;

[Collection(MediatorFactoryCollection.CollectionName)]
public class MembersAppTests(MediatorFactory mediatorFactory) : IAsyncLifetime
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    private GymDto _gym = null!;
    private MemberDto _member = null!;
    private RoomDto _room = null!;
    private MemberDto _trainerMember = null!;
    private TrainerDto _trainer = null!;
    private SubscriptionDto _subscription = null!;
    private SessionDto _session = null!;
    private BookingDto _booking = null!;
    private int _roomCapacity = 2;
    
    public async Task InitializeAsync()
    {
        _gym = await CreateGym();
        _room = await CreateRoom(_gym.Id, "Room 1", _roomCapacity);
        _member = await CreateMember(_gym.Id);
        _trainerMember = await CreateMember(_gym.Id, "Trainer 1");
        _trainer = await CreateTrainer(_trainerMember.Id);
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    [Fact]
    public async Task Create_WhenCommandIsValid_CreateWithSucess()
    {        
        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: _gym.Id,
                                                                 userName: "memberUserName",
                                                                 password: "Abc123");
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();        

        var queryResult = await _mediator.Send(new GetMemberQuery(Id: result.Value));
        queryResult.IsError.Should().BeFalse();
        var member = queryResult.Value;
        // Assert
        member.UserName.Should().BeEquivalentTo(command.UserName);
        member.Id.Should().Be(result.Value);
    }

    private async Task<GymDto> CreateGym(string? name = null, string? address = null)
    {
        var gymName = name ?? Constants.Gyms.Name;
        var gymAddress = address ?? Constants.Gyms.Address;

        var command = GymCommandFactory.GetCreateGymCommand(gymName, gymAddress);
        var createGymResult = await _mediator.Send(command);
        createGymResult.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetGymQuery(Id: createGymResult.Value));

        // Assert
        queryResult.IsError.Should().BeFalse();

        return queryResult.Value;
    }

    private async Task<MemberDto> CreateMember(Guid gymId, string userName = "member1")
    {
        var createMemberCommand = MemberCommandFactory.GetCreateMemberCommand(gymId: gymId,
                                                                            userName: userName,
                                                                            password: "Abc123");
        var createMemberResult = await _mediator.Send(createMemberCommand);
        createMemberResult.IsError.Should().BeFalse();
        createMemberResult.Value.Should().NotBeEmpty();

        var queryResult = await _mediator.Send(new GetMemberQuery(Id: createMemberResult.Value));
        queryResult.IsError.Should().BeFalse();

        // Assert
        return queryResult.Value;
    }
    

    [Fact]
    public async Task Create_WhenGymNotExist_ShouldReturnGymNotFoundError()
    {
        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: Guid.NewGuid(),
                                                                userName: "UserName1",
                                                                password: "Abc123");
        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(GymErrors.GymNotFound(command.GymId).Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Create_WithInvalidUserName_ShouldReturnValidationError(int userNameLength)
    {
        var userName = new string('a', userNameLength);

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: _gym.Id,
                                                                userName: userName,
                                                                password: "Abc123");
        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("UserName"); 
    }

    [Fact]
    public async Task Create_WhenUserNameAlreadyExists_ShouldReturnValidationError()
    {

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: _gym.Id,
                                                                userName: "member1",
                                                                password: "123User1");

        var result2 = await _mediator.Send(command);

        // Assert
        result2.IsError.Should().BeTrue();
        result2.FirstError.Description.Should().Contain("already exist");
    }

    [Fact]
    public async Task Update_WhenCommandIsValid_ShouldUpdateWithSucess()
    {
        var command = new UpdateMemberCommand(Id: _member.Id, UserName: "Member-Two", Password: "Abc123");
        var result = await _mediator.Send(command);


        var queryResult = await _mediator.Send(new GetMemberQuery(Id: result.Value));
        queryResult.IsError.Should().BeFalse();
        var member = queryResult.Value;

        // Assert
        member.UserName.Should().BeEquivalentTo(command.UserName);
    }

    [Fact]
    public async Task Update_MemberDontExist_ReturnNotFoundError()
    {
        var command = new UpdateMemberCommand(Id: Guid.NewGuid(), UserName: "Member-Two", Password: "Abc123");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound(command.Id));

    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Update_WithInvalidUserName_ShouldReturnValidationError(int userNameLength)
    {
        var userName = new string('a', userNameLength);

        var command = new UpdateMemberCommand(Id: _member.Id, UserName: userName, Password: "Abc123");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("UserName");
    }

    [Fact]
    public async Task Update_WhenUserNameAlreadyExists_ShouldReturnValidationError()
    {
       var member2 = await CreateMember(_gym.Id, "member2");

        var command = new UpdateMemberCommand(member2.Id, UserName: "member1", Password: "123User1");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("already used");
    }

    [Fact(Skip = "UpdateAdminUser")]
    public async Task Update_WhenAdminUserNameNameAlreadyExists_ShouldReturnValidationError()
    {

        var command = new UpdateMemberCommand(Guid.Parse("7d555faf-06b9-409f-a3ba-60d2a6bfc228"), UserName: "admin", Password: "123User1");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Description.Should().Contain("already used");
    }

    [Fact]
    public async Task Delete_WhenCommandIsValid_ShouldDeleteWithSucess()
    {
        var command = new DeleteMemberCommand(Id: _member.Id);
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetMemberQuery(Id: command.Id));

        // Assert
        queryResult.IsError.Should().BeTrue();
        queryResult.FirstError.Should().Be(MemberErrors.MemberNotFound(command.Id));
    }

    [Fact]
    public async Task Delete_WhenMemberDontExist_ReturnNotFoundError()
    {
        var command = new DeleteMemberCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound(command.Id));
    }

    [Fact]
    public async Task Delete_WhenMemberHasBookings_ReturnCannotRemoveMemberWithBookingError()
    {

        _subscription = await CreateSubscription(SubscriptionType.Plus, _member.Id);
        _session = await CreateSession(_room.Id, _trainer.Id, "Session 1");
        _booking = await CreateBooking(_session.Id, _member.Id);

        var command = new DeleteMemberCommand(Id: _member.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.CannotRemoveMemberWithBooking(command.Id));
    }
    
    [Fact]
    public async Task Delete_WhenMemberHasSubscription_ReturnCannotDeleteMemberWithSubscriptionError()
    {

        _subscription = await CreateSubscription(SubscriptionType.Plus, _member.Id);
        _session = await CreateSession(_room.Id, _trainer.Id, "Session 1");
        // _booking = await CreateBooking(_session.Id, _member.Id);

        var command = new DeleteMemberCommand(Id: _member.Id);
        var result = await _mediator.Send(command);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.CannotDeleteMemberWithSubscription(command.Id));        
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
    public async Task ListMembers_ReturnCorrectMembers()
    {
        // Act
        var listResult = await _mediator.Send(new ListMembersQuery());
        listResult.IsError.Should().BeFalse();
        var members = listResult.Value.ToList();

        // Assert        
        members.Count().Should().Be(3);
        members.Should().BeOfType<List<MemberDto>>();
    }    

    [Fact]
    public async Task ListMembersByGym_ReturnCorrectMembers()
    {
        // Arrange 
        var gym2 = await CreateGym("Gym-Two", "Luanda");
        
        await CreateMember(gym2.Id, "Member2");
        await CreateMember(gym2.Id, "Member3");
        await CreateMember(gym2.Id, "Member4");
        
        // Act
        var list1Result = await _mediator.Send(new ListMembersByGymQuery(GymId: _gym.Id));
        list1Result.IsError.Should().BeFalse();
        var members1 = list1Result.Value.ToList();

        var list2Result = await _mediator.Send(new ListMembersByGymQuery(GymId: gym2.Id));
        list2Result.IsError.Should().BeFalse();
        var members2 = list2Result.Value.ToList();

        // Assert        
        members1.Count().Should().Be(2); // Admin dont have a gym associated
        members1.Should().BeOfType<List<MemberDto>>();

        members2.Count().Should().Be(3);
        members2.Should().BeOfType<List<MemberDto>>();
    }
}