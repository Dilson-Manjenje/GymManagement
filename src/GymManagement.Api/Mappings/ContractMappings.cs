
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Application.Trainers.Queries.Dtos;
using GymManagement.Contracts.Bookings;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Rooms;
using GymManagement.Contracts.Sessions;
using GymManagement.Contracts.Subscriptions;
using GymManagement.Contracts.Trainers;

namespace GymManagement.Api.Mappings;

public static class ContractMappings
{
	public static RoomResponse MapToRoomResponse(RoomDto room)
	{
		return new RoomResponse(Id: room.Id,
								Name: room.Name,
								Capacity: room.Capacity,
								IsAvailable: room.IsAvailable,
								GymId: room.GymId,
								GymName: room.GymName);
	}
	public static TrainerResponse MapToTrainerResponse(TrainerDto trainer)
	{
		return new TrainerResponse(Id: trainer.Id,
								Name: trainer.Name,
								Phone: trainer.Phone,
								Email: trainer.Email,
								Specialization: trainer.Specialization,
								IsActive: trainer.IsActive,
								GymId: trainer.GymId,
								GymName: trainer.GymName,
								MemberId: trainer.MemberId);
	}
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
								  Rooms: subscription.Rooms,
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

	public static SessionResponse MapToSessionResponse(SessionDto session)
	{
		return new SessionResponse(Id: session.Id,
									Title: session.Title,
									GymName: session.GymName,
									GymId: session.GymId,
									RoomId: session.RoomId,
									RoomName: session.RoomName,
									TrainerId: session.TrainerId,
									Trainer: session.TrainerName,
									Capacity: session.Capacity,
									Vacancy: session.Vacancy,
									StartDate: session.StartDate,
									EndDate: session.EndDate,
									Status: Enum.Parse<SessionStatusType>(session.Status.Name));
	}

	public static BookingResponse MapToBookingResponse(BookingDto booking)
    {
        return new BookingResponse(Id: booking.Id,
								   Title: booking.Title,
								   MemberId: booking.MemberId,
								   MemberName: booking.MemberName,
								   RoomId: booking.RoomId,
								   RoomName: booking.RoomName,
								   TrainerId: booking.TrainerId,
								   TrainerName: booking.TrainerName,
								   StartDate: booking.StartDate,
								   EndDate: booking.EndDate,
								   Status: Enum.Parse<BookingStatusType>(booking.Status.Name),
								   SessionId: booking.SessionId);

    }
}
