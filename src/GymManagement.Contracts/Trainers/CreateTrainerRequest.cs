namespace GymManagement.Contracts.Trainers;

// public record AddTrainerRequest(
//     Guid TrainerId);

public abstract record TrainerBaseRequest(string Name,
                                string Phone,
                                string Email,
                                string Specialization);    

public sealed record CreateTrainerRequest(string Name,
                                string Phone,
                                string Email,
                                string Specialization,
                                Guid AdminId): TrainerBaseRequest(Name: Name,
                                                                  Phone: Phone,
                                                                  Email: Email,
                                                                  Specialization: Specialization); 
