using ErrorOr;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.DeleteTrainer;
public record DeleteTrainerCommand(Guid Id) : IRequest<ErrorOr<Unit>>;