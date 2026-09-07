using ErrorOr;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.FinalizeSession;

public record FinalizeSessionCommand(Guid Id) : IRequest<ErrorOr<Guid>>;
