using ErrorOr;
using MediatR;

namespace GymManagement.Application.Members.Commands.CreateMember;

public sealed record CreateMemberCommand(string UserName,
                                         string Password,
                                         Guid GymId) : IRequest<ErrorOr<Guid>>;