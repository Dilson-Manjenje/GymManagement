using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Members;

namespace GymManagement.Application.Members.Queries.Dtos;

public sealed record MemberDto(Guid Id,
                               string UserName,
                               Guid? UserId,
                               Guid? GymId,
                               string? GymName)

{
    public static MemberDto MapToDto(Member member)
    {
        return new MemberDto(Id: member.Id,
                             UserName: member.UserName,
                             UserId: member.UserId,
                             GymId: member.GymId,
                             GymName: member.Gym?.Name);
    }
}
