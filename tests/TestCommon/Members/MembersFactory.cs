using GymManagement.Domain.Members;
using TestCommon.TestConstants;

namespace TestCommon.Members;

public static class MembersFactory
{
    public static Member CreateMember(string userName = Constants.Members.UserName,
                                  Guid? gymId = null)
    {
        return new Member(
            userName: userName,
            gymId: gymId ?? Constants.Gyms.NewId);
    }
}