using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Sessions.Shared;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.CreateSession;

public record CreateSessionCommand(Guid RoomId,
                                   Guid TrainerId,
                                   string Title,
                                   DateTime? StartDate = null,
                                   DateTime? EndDate = null) : SessionBaseCommand(RoomId,
                                                                                  TrainerId,
                                                                                  Title,
                                                                                  StartDate,
                                                                                  EndDate);
