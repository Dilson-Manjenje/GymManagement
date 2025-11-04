using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.Shared;

public abstract record RoomBaseCommand(string Name,
                                        int Capacity,
                                        Guid GymId) : IRequest<ErrorOr<Room>>;