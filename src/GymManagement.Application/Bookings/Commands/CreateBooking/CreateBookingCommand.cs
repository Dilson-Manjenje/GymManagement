using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;

namespace GymManagement.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(Guid SessionId, Guid MemberId) : IRequest<ErrorOr<Guid>>;
