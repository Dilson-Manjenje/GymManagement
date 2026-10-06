using GymManagement.Application.SubcutaneousTests.Common;
using MediatR;
using TestCommon.Gyms;
using FluentAssertions;
using GymManagement.Domain.Members;
using TestCommon.Members;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Members.Queries.Dtos;
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
using GymManagement.Application.Trainers.Commands.CreateTrainer;
using GymManagement.Domain.Trainers;
using GymManagement.Application.Trainers.Commands.UpdateTrainer;
using GymManagement.Application.Trainers.Commands.DeleteTrainer;
using GymManagement.Application.Trainers.Queries.ListTrainers;
using GymManagement.Application.Trainers.Queries.ListTrainersByGym;

namespace GymManagement.Application.SubcutaneousTests.Trainers;

[Collection(MediatorFactoryCollection.CollectionName)]
public class TrainerAppTests(MediatorFactory mediatorFactory) : IAsyncLifetime
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    private GymDto _gym = null!;
    private MemberDto _member = null!;
    private RoomDto _room = null!;
    private MemberDto _trainerMember = null!;
    private TrainerDto _trainer = null!;
    private SubscriptionDto _subscription = null!;
    private int _roomCapacity = 2;
    public async Task InitializeAsync()
    {
        _gym = await CreateGym();
        _room = await CreateRoom(_gym.Id, "Room 1", _roomCapacity);
        _member = await CreateMember(_gym.Id);
        _trainerMember = await CreateMember(_gym.Id, "Trainer 1");
        _trainer = await CreateTrainer(_trainerMember.Id);
        await CreateSession(_room.Id, _trainer.Id, "Session 1");
    }

    public Task DisposeAsync() => Task.CompletedTask;

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


    private async Task<TrainerDto> CreateTrainer(Guid memberId,
                                                 string name = "Trainer 1",
                                                 string phone = "923000001",
                                                 string email = "trainer1@gmail.com")
    {
        var command = TrainerCommandFactory.GetCreateTrainerCommand(memberId: memberId,
                                                                    name: name,
                                                                    phone: phone,
                                                                    email: email,
                                                                    specialization: "Body Build");

        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: result.Value));


        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

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
    public async Task Create_WhenCommandIsValid_CreateWithSucess()
    {

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: _trainer.Id));
        queryResult.IsError.Should().BeFalse();
        var trainer = queryResult.Value;
        // Assert
        trainer.Name.Should().BeEquivalentTo(_trainer.Name);
        trainer.Id.Should().Be(_trainer.Id);
    }

    [Fact]
    public async Task Create_WhenMemberDontExist_ReturnNotFoundError()
    {
        var command = new CreateTrainerCommand(Name: "Trainer 2",
                                               Phone: "123456789",
                                               Email: "trainer2@gmail.com",
                                               Specialization: "body build",
                                               MemberId: Guid.NewGuid());

        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound(command.MemberId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Create_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var name = new string('a', nameLength);
        var command = new CreateTrainerCommand(Name: name,
                                               Phone: "123456789",
                                               Email: $"{name}@gmail.com",
                                               Specialization: "body build",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(101)]
    public async Task Create_WithInvalidSpecializationName_ReturnValidationError(int nameLength)
    {
        var specalization = new string('a', nameLength);
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "123456789",
                                               Email: $"pt2@gmail.com",
                                               Specialization: specalization,
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Specialization");
    }

    [Fact]
    public async Task Create_WhenPhoneAlreadyExist_ReturnValidationError()
    {
        var command = new CreateTrainerCommand(Name: "Trainer 2",
                                               Phone: "923000001",//_trainerPhone,
                                               Email: "trainer2@gmail.com",
                                               Specialization: "body build",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerPhoneAlreadyExists(command.Phone));
    }


    [Fact]
    public async Task Create_WhenEmailAlreadyExist_ReturnValidationError()
    {
        var command = new CreateTrainerCommand(Name: "Trainer 2",
                                               Phone: "923000111",
                                               Email: "trainer1@gmail.com",
                                               Specialization: "body build",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerEmailAlreadyExists($"{command.Email}"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(13)]
    public async Task Create_WithInvalidPhoneNumber_ReturnValidationError(int phoneLength)
    {
        var phoneNumber = new string('9', phoneLength);
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: phoneNumber,
                                               Email: $"pt2@gmail.com",
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Phone");
    }

    [Fact]
    public async Task Create_WithPhoneIsNotAllDigits_ReturnValidationError()
    {
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "00011199A",
                                               Email: $"pt2@gmail.com",
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Phone");
    }

    [Fact]
    public async Task Create_WithInvalidEmail_ReturnValidationError()
    {
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: $"pt2-gmail.com",
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email");
        result.FirstError.Description.Should().Contain("valid email is required");
    }

    
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(101)]
    public async Task Create_WithInvalidEmailSize_ReturnValidationError(int emailLength)
    {
        var email = new string('a', emailLength);

        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                                  Phone: "923999111",
                                                  Email: $"{email}",
                                                  Specialization: "Body Builder",
                                                  MemberId: _member.Id);


        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email");
        result.FirstError.Description.Should().Contain("valid email is required");
    }

    [Fact]
    public async Task Create_WhenEmailIsNull_CreateTrainer()
    {
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WhenNullEmailAlreadyExist_CreateTrainer()
    {
        var command = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var member3 = await CreateMember(_gym.Id, "pt3");
        var command3 = new CreateTrainerCommand(Name: "Personal Trainer 3",
                                               Phone: "923222111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: member3.Id);

        var result3 = await _mediator.Send(command3);

        // Assert
        result3.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WhenTrainerAlreadyInTheGym_ReturnValidationError()
    {
        var command = new CreateTrainerCommand(Name: "Trainer 2",
                                               Phone: "923000111",
                                               Email: "pt2@gmail.com",
                                               Specialization: "body build",
                                               MemberId: _trainerMember.Id);

        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerAlreadyAddedToGym(command.MemberId));
    }

    [Fact]
    public async Task Create_WhenMemberDontHaveGym_ReturnGymNotFoundError()
    {
        // Arrange 
        var queryResult = await _mediator.Send(new GetMemberQuery(Id: Guid.Parse("7d555faf-06b9-409f-a3ba-60d2a6bfc228")));
        queryResult.IsError.Should().BeFalse();
        var member = queryResult.Value;

        var command = new CreateTrainerCommand(Name: "Trainer 2",
                                               Phone: "923000111",
                                               Email: "pt2@gmail.com",
                                               Specialization: "body build",
                                               MemberId: member.Id);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberDontHaveGym(member.Id, member.UserName));

    }

    [Fact]
    public async Task Update_WhenCommandIsValid_UpdateWithSucess()
    {
        var command = new UpdateTrainerCommand(Id: _trainer.Id, Name: "Trainer 2", Phone: "923000001",
        Specialization: "KickBoxing", _gym.Id, "trainer2@gmail.com");

        var result = await _mediator.Send(command);

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: result.Value));
        queryResult.IsError.Should().BeFalse();
        var trainer = queryResult.Value;

        // Assert
        trainer.Name.Should().BeEquivalentTo(command.Name);
        trainer.Specialization.Should().BeEquivalentTo(command.Specialization);
        trainer.Phone.Should().BeEquivalentTo(command.Phone);
        trainer.Email.Should().BeEquivalentTo(command.Email);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task Update_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var name = new string('a', nameLength);
        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                               Name: name,
                                               Phone: "123456789",
                                               Email: $"{name}@gmail.com",
                                               Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(101)]
    public async Task Update_WithInvalidSpecializationName_ReturnValidationError(int nameLength)
    {
        var specalization = new string('a', nameLength);
        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                               Name: "Personal Trainer 2",
                                               Phone: "123456789",
                                               Email: $"pt2@gmail.com",
                                               Specialization: specalization);

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Specialization");
    }


    [Fact]
    public async Task Update_TrainerDontExist_ReturnNotFoundError()
    {
        var command = new UpdateTrainerCommand(Id: Guid.NewGuid(), Name: "Trainer 2", Phone: "923000001",
       Specialization: "KickBoxing", _gym.Id, "trainer2@gmail.com");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerNotFound(command.Id));

    }

    [Fact]
    public async Task Update_WhenPhoneAlreadyUseByOther_ReturnValidationError()
    {
        var member = await CreateMember(_gym.Id, "Member 2");
        var trainer2 = await CreateTrainer(member.Id, "Trainer 2", "923111000", "pt2@gmail.com");
        var command = new UpdateTrainerCommand(Id: trainer2.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: "923000001",
                                             Email: $"personal2@gmail.com",
                                             Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerPhoneAlreadyExists(command.Phone));
    }
    
    [Fact]
    public async Task Update_WhenEmailAlreadyUseByOther_ReturnValidationError()
    {
        var member = await CreateMember(_gym.Id, "Member 2");
        var trainer2 = await CreateTrainer(member.Id, "Trainer 2", "923111000", "pt2@gmail.com");
        var command = new UpdateTrainerCommand(Id: trainer2.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: "923111002",
                                             Email: $"trainer1@gmail.com",
                                             Specialization: "body build");
                                               
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerEmailAlreadyExists(command.Email!));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(13)]
    public async Task Update_WithInvalidPhoneNumber_ReturnValidationError(int phoneLength)
    {
        var phoneNumber = new string('9', phoneLength);

        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: phoneNumber,
                                             Email: $"trainer1@gmail.com",
                                             Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Phone");
    }

    [Fact]
    public async Task Update_WithPhoneIsNotAllDigits_ReturnValidationError()
    {

        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: "00011199A",
                                             Email: $"trainer1@gmail.com",
                                             Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Phone");
    }

    [Fact]
    public async Task Update_WithInvalidEmail_ReturnValidationError()
    {
        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: "000111999",
                                             Email: "pt2-gmail.com",
                                             Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email");
        result.FirstError.Description.Should().Contain("valid email is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(101)]
    public async Task Update_WithInvalidEmailSize_ReturnValidationError(int emailLength)
    {
        var email = new string('a', emailLength);

        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                             Name: "Personal Trainer 2",
                                             Phone: "923000111",
                                             Email: $"{email}",
                                             Specialization: "body build");

        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email");
        result.FirstError.Description.Should().Contain("valid email is required");
    }

    [Fact]
    public async Task Update_WhenEmailIsNull_UpdateWithSucess()
    {

        var command = new UpdateTrainerCommand(Id: _trainer.Id,
                                         Name: "Personal Trainer 2",
                                         Phone: "923000111",
                                         Email: null,
                                         Specialization: "body build");
        
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Update_WithNullEmailWhenAlreadyExist_UpdateWithSucess()
    {
        var command2 = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var result2 = await _mediator.Send(command2);
        result2.IsError.Should().BeFalse();

        var member3 = await CreateMember(_gym.Id, "pt3");
        var command3 = new CreateTrainerCommand(Name: "Personal Trainer 3",
                                               Phone: "923222111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: member3.Id);
        var result3 = await _mediator.Send(command3);
        result3.IsError.Should().BeFalse();

        var updateCmd = new UpdateTrainerCommand(Id: result3.Value,
                                         Name: command3.Name,
                                         Phone: command3.Phone,
                                         Email: null,
                                         Specialization: command3.Specialization);

        var result = await _mediator.Send(updateCmd);                                         
                                                 
        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_WhenCommandIsValid_DeleteTrainer()
    {
         var createTrainerCmd = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var createResult = await _mediator.Send(createTrainerCmd);
        createResult.IsError.Should().BeFalse();
        var trainerId = createResult.Value;
        
        var result = await _mediator.Send(new DeleteTrainerCommand(Id: trainerId));

        result.IsError.Should().BeFalse();

        var queryResult = await _mediator.Send(new GetTrainerQuery(Id: trainerId));

        // Assert
        queryResult.IsError.Should().BeTrue();
        queryResult.FirstError.Should().Be(TrainerErrors.TrainerNotFound(trainerId));
    }

    [Fact]
    public async Task Delete_WhenTrainer_ReturnNotFoundError()
    {
        var command = new DeleteTrainerCommand(Id: Guid.NewGuid());
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.TrainerNotFound(command.Id));
    }

    [Fact]
    public async Task Delete_WhenTrainerHasSession_ReturnCannotRemoveTrainerWithSessionError()
    {
        var command = new DeleteTrainerCommand(Id: _trainer.Id);
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TrainerErrors.CantRemoveTrainerWithSession(command.Id));        
    }


    [Fact]
    public async Task ListMembers_ReturnCorrectMembers()
    {
        var createTrainerCmd = new CreateTrainerCommand(Name: "Personal Trainer 2",
                                               Phone: "923999111",
                                               Email: null,
                                               Specialization: "Body Builder",
                                               MemberId: _member.Id);

        var createResult = await _mediator.Send(createTrainerCmd);
        createResult.IsError.Should().BeFalse();

        // Act
        var listResult = await _mediator.Send(new ListTrainersQuery());
        listResult.IsError.Should().BeFalse();
        var trainers = listResult.Value.ToList();

        // Assert        
        trainers.Count().Should().Be(2);
        trainers.Should().BeOfType<List<TrainerDto>>();
    }    

    [Fact]
    public async Task ListTrainersByGym_ReturnCorrectMembers()
    {
        // Arrange 
        await CreateTrainer(_member.Id, "Trainer 2", "923001001", "trainer2@gmail.com");
        // var createTrainerCmd = new CreateTrainerCommand(Name: "Personal Trainer 2",
        //                                        Phone: "923999111",
        //                                        Email: null,
        //                                        Specialization: "Body Builder",
        //                                        MemberId: _member.Id);
        // var createResult = await _mediator.Send(createTrainerCmd);
        // createResult.IsError.Should().BeFalse();

        var gym2 = await CreateGym("Gym-Two", "Luanda");
        var member3 = await CreateMember(gym2.Id, "Member3");
        await CreateTrainer(member3.Id, "Trainer 3", "923001002", "trainer3@gmail.com");
        var member4 = await CreateMember(gym2.Id, "Member4");
        await CreateTrainer(member4.Id, "Trainer 4", "923001004", "trainer4@gmail.com");

        // Act
        var list1Result = await _mediator.Send(new ListTrainersByGymQuery(GymId: _gym.Id));
        list1Result.IsError.Should().BeFalse();
        var trainers1 = list1Result.Value.ToList();

        var list2Result = await _mediator.Send(new ListTrainersByGymQuery(GymId: gym2.Id));
        list2Result.IsError.Should().BeFalse();
        var trainers2 = list2Result.Value.ToList();

        // Assert        
        trainers1.Count().Should().Be(2); 
        trainers1.Should().BeOfType<List<TrainerDto>>();

        trainers2.Count().Should().Be(2);
        trainers2.Should().BeOfType<List<TrainerDto>>();
    }
}