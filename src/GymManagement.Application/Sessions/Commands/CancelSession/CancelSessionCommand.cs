using ErrorOr;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.CancelSession;

public record CancelSessionCommand(Guid Id) : IRequest<ErrorOr<Guid>>;
