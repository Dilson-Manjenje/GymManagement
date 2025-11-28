namespace GymManagement.Contracts.Trainers;

public sealed record TrainerResponse(
                                Guid Id,
                                string Name,
                                string Phone,
                                //string Email,
                                string Specialization,
                                Guid GymId,
                                string GymName,
                                Guid AdminId);
