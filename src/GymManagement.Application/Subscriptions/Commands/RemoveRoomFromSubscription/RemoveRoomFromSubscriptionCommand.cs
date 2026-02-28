using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.RemoveRoomFromSubscription;

public record RemoveRoomFromSubscriptionCommand(Guid SubscriptionId, Guid RoomId) : IRequest<ErrorOr<Unit>>;