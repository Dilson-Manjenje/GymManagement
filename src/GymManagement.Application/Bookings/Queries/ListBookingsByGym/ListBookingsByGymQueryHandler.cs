using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.ListBookingsByGym;

public class ListBookingsByGymQueryHandler : IRequestHandler<ListBookingsByGymQuery, ErrorOr<IEnumerable<BookingDto>?>>
{
    private readonly IBookingsRepository _bookingsRepository;

    public ListBookingsByGymQueryHandler(IBookingsRepository bookingsRepository)
    {
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<IEnumerable<BookingDto>?>> Handle(ListBookingsByGymQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _bookingsRepository.ListByGymAsync(query.GymId);

        if (bookings is null || !bookings.Any())
            return new List<BookingDto>();

        return bookings?.Select(booking => BookingDto.MapToDto(booking)).ToList();
        
    }
}
