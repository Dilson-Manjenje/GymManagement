using ErrorOr;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Commands.DeleteMember;

public sealed record DeleteMemberCommand(Guid MemberId): IRequest<ErrorOr<Unit>>;