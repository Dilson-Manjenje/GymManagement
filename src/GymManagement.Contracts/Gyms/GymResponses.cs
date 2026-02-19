using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Gyms;

public record GymResponse(Guid Id,
                          string Name,
                          string Address);
    
public record ListGymsResponse(IEnumerable<GymResponse> Data): ListResponse<GymResponse>(Data: Data);
