using ErrorOr;
using GymManagement.Application.Gyms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.GetGym;

public record GetGymQuery(Guid GymId): IRequest<ErrorOr<GymDto>>;
