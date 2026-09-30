using ErrorOr;
using GymManagement.Application.Trainers.Shared;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public sealed record CreateTrainerCommand(string Name,
                                          string Phone,
                                          string? Email,
                                          string Specialization,
                                          Guid MemberId) : TrainerBaseCommand(Name,
                                                                              Phone,
                                                                              Email,
                                                                              Specialization);