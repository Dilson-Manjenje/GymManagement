using ErrorOr;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.DeleteSession;
public record DeleteSessionCommand(Guid Id) : IRequest<ErrorOr<Unit>>;