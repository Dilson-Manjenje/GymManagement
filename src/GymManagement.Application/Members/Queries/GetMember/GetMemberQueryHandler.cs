using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Queries.GetMember;

public class GetMemberQueryHandler : IRequestHandler<GetMemberQuery, ErrorOr<MemberDto>>
{
    private readonly IMembersRepository _membersRepository;

    public GetMemberQueryHandler(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<MemberDto>> Handle(GetMemberQuery query, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetByIdAsync(query.MemberId);
    
        return (member is null)
           ? MemberErrors.MemberNotFound(query.MemberId)
           : MemberDto.MapToDto(member);
    }
}