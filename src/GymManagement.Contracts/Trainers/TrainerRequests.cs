namespace GymManagement.Contracts.Trainers;

public sealed record CreateTrainerRequest(string Name,
                                          string Phone,
                                          string? Email,
                                          string Specialization,
                                          Guid MemberId);

public sealed record UpdateTrainerRequest(string Name,
                                          string Phone,
                                          string? Email,
                                          string Specialization);                                    