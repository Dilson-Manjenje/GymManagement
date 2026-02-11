using FluentValidation;
using GymManagement.Application.Sessions.Shared;

namespace GymManagement.Application.Sessions.Commands.CreateSession;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        Include(new SessionBaseValidator());
    }
}