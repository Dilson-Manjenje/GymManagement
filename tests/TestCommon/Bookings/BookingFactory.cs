using ErrorOr;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Members;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Subscriptions;

namespace TestCommon.Bookings;

public static class BookingFactory
{
    public static ErrorOr<Booking> CreateBooking(Session session, Member member, Subscription subscription)
    {
        var booking = Booking.Create(member: member, session: session, subscription);

        if (booking.IsError)
            return booking.Errors;

        return booking.Value;
    }
}