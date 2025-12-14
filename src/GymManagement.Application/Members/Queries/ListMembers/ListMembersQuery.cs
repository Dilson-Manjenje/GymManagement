using ErrorOr;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembers;

public record ListMembersQuery(): IRequest<ErrorOr<IEnumerable<Member>?>>;
