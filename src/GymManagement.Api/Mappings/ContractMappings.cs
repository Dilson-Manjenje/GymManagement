
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Subscriptions;

namespace GymManagement.Api.Mappings;

public static class ContractMappings
{
	public static SubscriptionResponse MapToSubscriptionResponse(SubscriptionDto subscription)
	{
		return new SubscriptionResponse(Id: subscription.Id,
								  SubscriptionType: Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name),
								  Price: subscription.Price,
								  StartDate: subscription.StartDate,
								  EndDate: subscription.EndDate,
								  IsActive: subscription.IsActive,
								  GymName: subscription.GymName,
								  MaxRooms: subscription.MaxRooms,
								  //Rooms: null,
								  MaxDailySessions: subscription.MaxDailySessions,
								  MemberId: subscription.MemberId,
								  UserName: subscription.UserName);
	}

	public static MemberResponse MapToMemberResponse(MemberDto member)
	{
		return new MemberResponse(Id: member.Id,
								UserName: member.UserName,
								UserId: member.UserId,
								GymId: member.GymId,
								GymName: member.GymName);
	}
}
