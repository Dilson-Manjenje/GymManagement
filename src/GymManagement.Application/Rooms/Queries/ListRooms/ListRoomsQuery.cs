using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRooms;

public record ListRoomsQuery(): IRequest<ErrorOr<IEnumerable<Room>?>>;
