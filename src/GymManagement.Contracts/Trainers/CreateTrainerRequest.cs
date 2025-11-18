namespace GymManagement.Contracts.Trainers;

// public record AddTrainerRequest(
//     Guid TrainerId);

public abstract record TrainerBaseRequest(string Name,
                                string Phone,
                                string Email,
                                string Specialization,
                                Guid GymId);    

public sealed record CreateTrainerRequest(string Name,
                                string Phone,
                                string Email,
                                string Specialization,
                                Guid GymId,
                                Guid AdminId): TrainerBaseRequest(Name: Name,
                                                                  Phone: Phone,
                                                                  Email: Email,
                                                                  Specialization: Specialization, GymId: GymId); 
