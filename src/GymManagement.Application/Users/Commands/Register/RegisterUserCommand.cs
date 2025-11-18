using ErrorOr;
using GymManagement.Domain.Admins;
using MediatR;

namespace GymManagement.Application.Users.Commands.Register;

public sealed record RegisterUserCommand(string UserName, string Password): IRequest<ErrorOr<Admin>>;