using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Members.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembersByGym;

public class ListMembersByGymQueryHandler : IRequestHandler<ListMembersByGymQuery, ErrorOr<IEnumerable<MemberDto>?>>
{
    private readonly IMembersRepository _membersRepository;

    public ListMembersByGymQueryHandler(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<IEnumerable<MemberDto>?>> Handle(ListMembersByGymQuery query, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.ListByGymAsync(gymId: query.GymId, cancellationToken);
        
        return members?.Select(member => MemberDto.MapToDto(member)).ToList();            
    }
}