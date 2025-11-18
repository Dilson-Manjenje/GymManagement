namespace GymManagement.Contracts.Users;

public record GetAdminResponse(Guid Id, string UserName, Guid? UserId, Guid? SubscriptionId);
