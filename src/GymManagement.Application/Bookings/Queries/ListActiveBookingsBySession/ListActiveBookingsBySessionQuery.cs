using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListActiveBookingsBySession;

public record ListActiveBookingsBySessionQuery(Guid SessionId): IRequest<ErrorOr<IEnumerable<BookingDto>?>>;
