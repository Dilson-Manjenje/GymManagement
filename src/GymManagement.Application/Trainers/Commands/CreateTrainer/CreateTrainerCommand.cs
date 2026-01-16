using ErrorOr;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public sealed record CreateTrainerCommand(string Name,
                                          string Phone,
                                          string? Email,
                                          string Specialization,
                                          Guid MemberId) : IRequest<ErrorOr<Guid>>;