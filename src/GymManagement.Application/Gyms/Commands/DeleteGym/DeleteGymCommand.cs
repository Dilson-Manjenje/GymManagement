using ErrorOr;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.DeleteGym;
public record DeleteGymCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;