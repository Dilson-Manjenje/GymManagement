using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Rooms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionRooms;

public sealed record ListRoomsInSubscriptionQuery(Guid SubscriptionId): IRequest<ErrorOr<IEnumerable<RoomDto>?>>;