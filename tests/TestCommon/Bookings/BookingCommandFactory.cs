using GymManagement.Application.Bookings.Commands.CreateBooking;

namespace TestCommon.Bookings;

public static class BookingCommandFactory
{
    public static CreateBookingCommand GetCreateBookingCommand(Guid sessionId, Guid memberId)
    {
        return new CreateBookingCommand(sessionId, memberId);
    }
}