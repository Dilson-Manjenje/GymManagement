using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Sessions;

public record SessionResponse(Guid Id,
                             string Title,
                             string? GymName,
                             Guid? GymId,
                             Guid RoomId,
                             string RoomName,
                             Guid TrainerId,
                             string? Trainer,
                             int Capacity,
                             int Vacancy,
                             DateTime StartDate,
                             DateTime EndDate,
                             SessionStatusType Status); 
public record ListSessionsResponse(IEnumerable<SessionResponse> Data) : ListResponse<SessionResponse>(Data: Data);                                                                             