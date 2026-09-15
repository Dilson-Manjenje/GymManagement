using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsByMember;

public record ListBookingsByMemberQuery(Guid MemberId): IRequest<ErrorOr<IEnumerable<BookingDto>?>>;
