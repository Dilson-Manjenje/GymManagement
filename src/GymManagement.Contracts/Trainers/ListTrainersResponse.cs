namespace GymManagement.Contracts.Trainers;

public record ListTrainersResponse(IEnumerable<TrainerResponse> Trainers);