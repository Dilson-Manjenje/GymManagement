using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Common;

namespace GymManagement.Domain.Bookings.Events;

public record BookingCreatedEvent(Guid BookingId, Guid SessionId): IDomainEvent;

public record BookingCanceledEvent(Guid BookingId, Guid SessionId): IDomainEvent;