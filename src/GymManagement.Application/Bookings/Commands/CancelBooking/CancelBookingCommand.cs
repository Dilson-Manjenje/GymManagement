using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Bookings;
using MediatR;

namespace GymManagement.Application.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(Guid BookingId) : IRequest<ErrorOr<Guid>>;
