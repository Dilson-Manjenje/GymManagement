using ErrorOr;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Commands.Register;

public sealed record RegisterUserCommand(string UserName, string Password, Guid GymId): IRequest<ErrorOr<Member>>;