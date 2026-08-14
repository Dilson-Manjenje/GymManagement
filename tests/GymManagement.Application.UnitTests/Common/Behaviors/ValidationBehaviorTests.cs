using ErrorOr;
using FluentAssertions;
using FluentValidation;
using GymManagement.Application.Common.Behaviors;
using GymManagement.Application.Gyms.Commands.CreateGym;
using MediatR;
using NSubstitute;
using TestCommon.Gyms;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace GymManagement.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange 

        // Create request 
        var createGymRequest = GymCommandFactory.CreateGymCommand();

        // Create next behavior (mock)
        var gym = GymFactory.CreateGym();
        var mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Guid>>>();
        mockNextBehavior.Invoke().Returns(gym.Id);

        // Create validator (mock)
        var mockValidator = Substitute.For<IValidator<CreateGymCommand>>();
        mockValidator
            .ValidateAsync(createGymRequest, Arg.Any<CancellationToken>())
            .Returns(new FluentValidationResult());

        // create validation behavior (SUT)
        var validationBehavior = new ValidationBehavior<CreateGymCommand, ErrorOr<Guid>>();

        // Act
        var result = await validationBehavior.Handle(createGymRequest, mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(gym.Id);
                
    }
}

