using ErrorOr;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.RemoveTrainer;
public record RemoveTrainerCommand(Guid Id) : IRequest<ErrorOr<Updated>>;