using ErrorOr;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Commands.UpdateMember;

public sealed record UpdateMemberCommand(Guid Id,
                                         string UserName,
                                         string? Password,
                                         Guid? GymId = null) : IRequest<ErrorOr<Guid>>;