namespace GymManagement.Contracts.Gyms;

public record ListGymsResponse(IEnumerable<GymResponse> Gyms);