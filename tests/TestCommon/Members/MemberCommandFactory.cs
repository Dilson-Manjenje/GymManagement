using GymManagement.Application.Members.Commands.CreateMember;

namespace TestCommon.Members;

public static class MemberCommandFactory
{
    public static CreateMemberCommand GetCreateMemberCommand(Guid gymId, string? userName = null, string? password = null )
    {
        return new CreateMemberCommand(UserName: userName!,
                                       Password: password!,
                                       GymId: gymId);
    }
}