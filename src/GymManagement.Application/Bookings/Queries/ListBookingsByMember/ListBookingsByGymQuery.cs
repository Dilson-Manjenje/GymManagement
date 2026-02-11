using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsByGym;

public record ListBookingsByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<BookingDto>?>>;
