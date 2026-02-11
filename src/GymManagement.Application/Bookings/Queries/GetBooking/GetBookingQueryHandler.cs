using ErrorOr;
using GymManagement.Application.Bookings.Queries.Dtos;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using MediatR;

namespace GymManagement.Application.Bookings.Queries.GetBooking;

public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, ErrorOr<BookingDto>>
{
    private readonly IBookingsRepository _bookingsRepository;
    
    public GetBookingQueryHandler(IBookingsRepository bookingsRepository)
    {
        _bookingsRepository = bookingsRepository;
    }

    public async Task<ErrorOr<BookingDto>> Handle(GetBookingQuery query, CancellationToken cancellationToken)
    {
        var booking = await _bookingsRepository.GetByIdAsync(query.BookingId);

        return (booking is null)
            ? BookingErrors.BookingNotFound(query.BookingId)
            : BookingDto.MapToDto(booking);
    }
}
