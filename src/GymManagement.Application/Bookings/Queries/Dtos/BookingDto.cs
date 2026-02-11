using GymManagement.Domain.Bookings;
using GymManagement.Domain.Sessions;

namespace GymManagement.Application.Bookings.Queries.Dtos;

public record BookingDto(Guid Id,
                         string Title,
                         Guid MemberId,
                         string MemberName,
                         Guid RoomId,
                         string RoomName,
                         Guid TrainerId,
                         string TrainerName,
                         DateTime StartDate,
                         DateTime EndDate,
                         BookingStatus Status,
                         Guid SessionId)
{
    public static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        (
            Id: booking.Id,
            MemberId: booking.MemberId,
            MemberName: booking.Member.UserName,
            Title: booking.Session.Title,
            RoomId: booking.Session.RoomId,
            RoomName: booking.Session.Room.Name,
            TrainerId: booking.Session.TrainerId,
            TrainerName: booking.Session.Trainer.Name,
            StartDate: booking.Session.StartDate,
            EndDate: booking.Session.EndDate,
            Status: booking.Status,
            SessionId: booking.SessionId);
    }

    
       
}                                    