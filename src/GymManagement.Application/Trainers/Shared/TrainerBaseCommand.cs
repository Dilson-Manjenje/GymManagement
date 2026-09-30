using ErrorOr;
using MediatR;

namespace GymManagement.Application.Trainers.Shared;

public abstract record TrainerBaseCommand(string Name,
                                          string Phone,
                                          string? Email,
                                          string Specialization) : IRequest<ErrorOr<Guid>>;
