using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembers;

public class ListMembersQueryHandler : IRequestHandler<ListMembersQuery, ErrorOr<IEnumerable<MemberDto>?>>
{
    private readonly IMembersRepository _membersRepository;

    public ListMembersQueryHandler(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<IEnumerable<MemberDto>?>> Handle(ListMembersQuery query, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.ListAsync();

        return members?.Select(member => MemberDto.MapToDto(member)).ToList();

    }
}