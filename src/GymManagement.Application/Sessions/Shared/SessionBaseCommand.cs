using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;

namespace GymManagement.Application.Sessions.Shared;
public abstract record SessionBaseCommand(Guid RoomId,
                                          Guid TrainerId,
                                          string Title,
                                          DateTime? StartDate = null,
                                          DateTime? EndDate = null) : IRequest<ErrorOr<Guid>>;