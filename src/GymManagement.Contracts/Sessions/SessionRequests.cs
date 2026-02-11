namespace GymManagement.Contracts.Sessions;

public sealed record CreateSessionRequest(Guid RoomId,
                                          Guid TrainerId,
                                          string Title,
                                          DateTime? StartDate = null,
                                          DateTime? EndDate = null);    
                                          
public sealed record UpdateSessionRequest(Guid Id,
                                          Guid RoomId,
                                          Guid TrainerId,
                                          string Title,
                                          DateTime? StartDate = null,
                                          DateTime? EndDate = null);                                              