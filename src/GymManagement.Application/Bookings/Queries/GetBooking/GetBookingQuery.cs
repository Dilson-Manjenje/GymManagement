using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.GetBooking;

public record GetBookingQuery(Guid BookingId): IRequest<ErrorOr<BookingDto>>;
