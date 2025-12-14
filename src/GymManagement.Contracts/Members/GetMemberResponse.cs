namespace GymManagement.Contracts.Members;

public record GetMemberResponse(Guid Id,
                                string UserName,
                                Guid? UserId,
                                Guid? CurrentSubscriptionId,
                                Guid? GymId,
                                string GymName);