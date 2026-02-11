using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookings;

public record ListBookingsQuery(): IRequest<ErrorOr<IEnumerable<BookingDto>?>>;
