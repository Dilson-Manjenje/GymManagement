using ErrorOr;
using GymManagement.Application.Members.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembersByGym;

public record ListMembersByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<MemberDto>?>>;
