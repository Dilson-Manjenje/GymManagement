using ErrorOr;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.UpdateTrainer;

public sealed record UpdateTrainerCommand(Guid Id,
                                          string Name,
                                          string Phone,
                                          string Specialization,
                                          Guid? GymId = null,
                                          string? Email = null) : IRequest<ErrorOr<Guid>>;