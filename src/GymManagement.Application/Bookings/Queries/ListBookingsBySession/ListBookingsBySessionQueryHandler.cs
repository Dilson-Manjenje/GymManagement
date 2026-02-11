using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsBySession;

public class ListBookingsBySessionQueryHandler : IRequestHandler<ListBookingsBySessionQuery, ErrorOr<IEnumerable<BookingDto>?>>
{
    private readonly IBookingsRepository _bookingsRepository;

    public ListBookingsBySessionQueryHandler(IBookingsRepository bookingsRepository)
    {
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<IEnumerable<BookingDto>?>> Handle(ListBookingsBySessionQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _bookingsRepository.ListActiveBookingsBySessionAsync(query.SessionId);

        if (bookings is null || !bookings.Any())
            return new List<BookingDto>();

        return bookings?.Select(booking => BookingDto.MapToDto(booking)).ToList();
        
    }
}
