using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GymManagement.Application.Common.Behaviors;
using GymManagement.Application.Gyms.Commands.CreateGym;
using MediatR;
using NSubstitute;
using TestCommon.Gyms;
//using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace GymManagement.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateGymCommand, ErrorOr<Guid>> _validationBehavior;
    private readonly IValidator<CreateGymCommand> _mockValidator;
    private readonly RequestHandlerDelegate<ErrorOr<Guid>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {                
        // Create next behavior (mock)
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Guid>>>();

        // Create validator (mock)
        _mockValidator = Substitute.For<IValidator<CreateGymCommand>>();
        
        // create validation behavior (SUT)
        _validationBehavior = new ValidationBehavior<CreateGymCommand, ErrorOr<Guid>>(_mockValidator);

    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange 
        var createGymRequest = GymCommandFactory.CreateGymCommand();
        var gym = GymFactory.CreateGym();

        _mockValidator
            .ValidateAsync(createGymRequest, Arg.Any<CancellationToken>())
            //.Returns(new FluentValidationResult());
            .Returns(new FluentValidation.Results.ValidationResult()); 

        _mockNextBehavior.Invoke().Returns(gym.Id);

        // Act
        var result = await _validationBehavior.Handle(createGymRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(gym.Id);

    }
    
    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange 
        var createGymRequest = GymCommandFactory.CreateGymCommand();
        List<ValidationFailure> validationFailures = [new(propertyName: "Name", errorMessage: "Name is required.")];

        var gym = GymFactory.CreateGym("","b");

        _mockValidator
            .ValidateAsync(createGymRequest, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(validationFailures));

        _mockNextBehavior.Invoke().Returns(gym.Id);

        // Act
        var result = await _validationBehavior.Handle(createGymRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
        result.FirstError.Description.Should().Be("Name is required.");
                
    }
}

