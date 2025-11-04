using ErrorOr;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.UpdateGym;
public record UpdateGymCommand(Guid Id,
                               string Name,
                               string Address) : IRequest<ErrorOr<Gym>>;
