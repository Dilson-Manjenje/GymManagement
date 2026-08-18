using MediatR;
using TestCommon.Subscriptions;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using ErrorOr;
using TestCommon.Gyms;

namespace GymManagement.Application.SubcutaneousTests.Gyms.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateGymTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateGym_WhenCommandIsValid_ShouldReturnGymId()
    {
        // Arrange 
        var gym = GymFactory.CreateGym();
        var createGymCommand = GymCommandFactory.CreateGymCommand(gym.Name, gym.Address);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateGym_WithInvalidName_ReturnError()
    {
        // Arrange 
        var createGymCommand = GymCommandFactory.CreateGymCommand(name: "a");

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
        result.FirstError.Description.Should().Contain("at least 3 characters");
    }

    [Fact]
    public async Task CreateGym_WithEmptyAddress_ReturnError()
    {
        // Arrange 
        var createGymCommand = GymCommandFactory.CreateGymCommand(name: "Quibuma", null);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Address");
        result.FirstError.Description.Should().Contain("obrigatório");
    }

    [Fact]
    public async Task CreateGym_WithAddressGreaterThanLimit_ReturnError()
    {
        // Arrange 
        string addr = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"+
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        var createGymCommand = GymCommandFactory.CreateGymCommand(name: "Quibuma", addr);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Address");
        result.FirstError.Description.Should().Contain("characteres maximum");
    }

}