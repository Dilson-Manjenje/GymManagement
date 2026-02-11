using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsBySession;

public record ListBookingsBySessionQuery(Guid SessionId): IRequest<ErrorOr<IEnumerable<BookingDto>?>>;
