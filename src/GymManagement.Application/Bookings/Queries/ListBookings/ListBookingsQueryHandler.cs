using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookings;

public class ListBookingsQueryHandler : IRequestHandler<ListBookingsQuery, ErrorOr<IEnumerable<BookingDto>?>>
{
    private readonly IBookingsRepository _bookingsRepository;

    public ListBookingsQueryHandler(IBookingsRepository bookingsRepository)
    {
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<IEnumerable<BookingDto>?>> Handle(ListBookingsQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _bookingsRepository.ListAsync();

        if (bookings is null || !bookings.Any())
            return new List<BookingDto>();

        return bookings?.Select(booking => BookingDto.MapToDto(booking)).ToList();
        
    }
}
