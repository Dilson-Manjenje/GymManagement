using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Trainers;

public sealed record TrainerResponse(
                                Guid Id,
                                string Name,
                                string Phone,
                                string? Email,
                                string Specialization,
                                bool IsActive,
                                Guid MemberId,
                                Guid GymId,
                                string? GymName = null);

public record ListTrainersResponse(IEnumerable<TrainerResponse> Data): ListResponse<TrainerResponse>(Data: Data);   
