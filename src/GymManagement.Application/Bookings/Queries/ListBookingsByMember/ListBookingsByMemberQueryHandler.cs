using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsByMember;

public class ListBookingsByMemberQueryHandler : IRequestHandler<ListBookingsByMemberQuery, ErrorOr<IEnumerable<BookingDto>?>>
{
    private readonly IBookingsRepository _bookingsRepository;

    public ListBookingsByMemberQueryHandler(IBookingsRepository bookingsRepository)
    {
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<IEnumerable<BookingDto>?>> Handle(ListBookingsByMemberQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _bookingsRepository.ListByMemberAsync(query.MemberId);

        if (bookings is null || !bookings.Any())
            return new List<BookingDto>();

        return bookings?.Select(booking => BookingDto.MapToDto(booking)).ToList();
        
    }
}
