namespace GymManagement.Contracts.Members;

public record ListMembersResponse(IEnumerable<GetMemberResponse> Members);