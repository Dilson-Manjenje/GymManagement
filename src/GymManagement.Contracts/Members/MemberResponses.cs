using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Members;

public record MemberResponse(Guid Id,
                                string UserName,
                                Guid? UserId,
                                Guid? GymId,
                                string GymName);

public record ListMembersResponse(IEnumerable<MemberResponse> Data): ListResponse<MemberResponse>(Data: Data);
