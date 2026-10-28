using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembersByGym;

public class ListMembersByGymQueryHandler : IRequestHandler<ListMembersByGymQuery, ErrorOr<IEnumerable<MemberDto>?>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IGymsRepository _gymsRepository;

    public ListMembersByGymQueryHandler(IMembersRepository membersRepository, IGymsRepository gymsRepository)
    {
        _membersRepository = membersRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<IEnumerable<MemberDto>?>> Handle(ListMembersByGymQuery query, CancellationToken cancellationToken)
    {
         var gym = await _gymsRepository.GetByIdAsync(query.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(query.GymId);

        var members = await _membersRepository.ListByGymAsync(gymId: query.GymId, cancellationToken);

        return members?.Select(member => MemberDto.MapToDto(member)).ToList();            
    }
}