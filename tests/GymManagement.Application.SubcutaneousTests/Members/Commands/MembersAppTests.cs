using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.SubcutaneousTests.Common;
using MediatR;
using TestCommon.Gyms;
using FluentAssertions;
using ErrorOr;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Members;
using TestCommon.Members;
using GymManagement.Domain.Subscriptions;
using TestCommon.TestConstants;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.Dtos;

namespace GymManagement.Application.SubcutaneousTests.Members;

[Collection(MediatorFactoryCollection.CollectionName)]
public class MembersAppTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateMember_WhenCommandIsValid_ShouldReturnGymId()
    {
        var gym = await CreateGym();

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: gym.Id,
                                                                            userName: "memberUserName",
                                                                            password: "Abc123");
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    private async Task<GymDto> CreateGym(string? name = null)
    {
        var gymName = name ?? Constants.Gyms.Name;
        var createGymCommand = GymCommandFactory.GetCreateGymCommand(gymName, Constants.Gyms.Address);
        var createGymResult = await _mediator.Send(createGymCommand);

        var gym = await _mediator.Send(new GetGymQuery(Id: createGymResult.Value));

        // Assert
        createGymResult.IsError.Should().BeFalse();
        createGymResult.Value.Should().NotBeEmpty();

        return gym.Value;
    }

    [Fact]
    public async Task CreateMember_WhenGymNotExist_ShouldReturnGymNotFoundError()
    {
        var gymId = Guid.NewGuid();

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: gymId,
                                                                userName: "UserName1",
                                                                password: "Abc123");
        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        //result.FirstError.Code.Should().Be("Gym.NotFound");
        result.FirstError.Code.Should().Be(GymErrors.GymNotFound(command.GymId).Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task CreateMember_WithInvalidUserName_ShouldReturnValidationError(int userNameLength)
    {
        var userName = new string('a', userNameLength);
        var gym = await CreateGym();

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: gym.Id,
                                                                userName: userName,
                                                                password: "Abc123");
        var result = await _mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("UserName"); 
    }

    [Fact]
    public async Task CreateMember_WhenUserNameAlreadyExists_ShouldReturnValidationError()
    {
        var gym = await CreateGym();

        var command = MemberCommandFactory.GetCreateMemberCommand(gymId: gym.Id,
                                                                userName: "UserName1",
                                                                password: "Abc123");
        var result = await _mediator.Send(command);
        result.IsError.Should().BeFalse();

        var command2 = MemberCommandFactory.GetCreateMemberCommand(gymId: gym.Id,
                                                                userName: "UserName1",
                                                                password: "123User1");

        var result2 = await _mediator.Send(command2);

        // Assert
        result2.IsError.Should().BeTrue();        
        result2.FirstError.Description.Should().Contain("already exist");        
    }
}