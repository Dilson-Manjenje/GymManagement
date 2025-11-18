namespace GymManagement.Contracts.Users;

public record ListAdminsResponse(IEnumerable<GetAdminResponse> Admins);