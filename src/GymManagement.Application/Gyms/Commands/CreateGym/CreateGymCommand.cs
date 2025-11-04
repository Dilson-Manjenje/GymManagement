using ErrorOr;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.CreateGym;
public record CreateGymCommand(string Name,
                               string Address) : IRequest<ErrorOr<Gym>>;