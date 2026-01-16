namespace GymManagement.Contracts.Trainers;

public sealed record TrainerRequest(string Name,
                                    string Phone,
                                    string? Email,
                                    string Specialization,
                                    Guid MemberId);