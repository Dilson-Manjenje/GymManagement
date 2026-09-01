using MediatR;
using GymManagement.Application.SubcutaneousTests.Common;
using FluentAssertions;
using TestCommon.Gyms;

namespace GymManagement.Application.SubcutaneousTests.Gyms;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GymAppTests(MediatorFactory mediatorFactory)
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

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(61)]
    public async Task CreateGym_WithInvalidName_ReturnValidationError(int nameLength)
    {
        var gymName = new string('a', nameLength);

        // Arrange 
        var createGymCommand = GymCommandFactory.CreateGymCommand(name: gymName, null);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }


    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(101)]
    public async Task CreateGym_WithInvalidAddress_ReturnValidationError(int addressLength)
    {        
        // Arrange 
        var address = new string('a', addressLength);
        var createGymCommand = GymCommandFactory.CreateGymCommand(name: "Quibuma", address);

        // Act
        var result = await _mediator.Send(createGymCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Address");
        //result.FirstError.Description.Should().Contain("is required");
    }

}