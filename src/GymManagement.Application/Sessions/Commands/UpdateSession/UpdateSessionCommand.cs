using GymManagement.Application.Sessions.Shared;

namespace GymManagement.Application.Sessions.Commands.UpdateSession;

public record UpdateSessionCommand(Guid Id,
                                   Guid RoomId,
                                   Guid TrainerId,
                                   string Title,
                                   DateTime? StartDate = null,
                                   DateTime? EndDate = null) : SessionBaseCommand(RoomId,
                                                                                  TrainerId,
                                                                                  Title,
                                                                                  StartDate,
                                                                                  EndDate);
