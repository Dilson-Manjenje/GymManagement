using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Trainers.Commands.CreateTrainer;
using GymManagement.Application.Trainers.Queries.ListTrainers;
using GymManagement.Application.Trainers.Commands.RemoveTrainer;


namespace GymManagement.Api.Controllers;

public class TrainersController : ApiBaseController
{
  private readonly ISender _mediator;
  public TrainersController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateTrainer(CreateTrainerRequest request)
  {
    var cmd = new CreateTrainerCommand(Name: request.Name,
                                       Phone: request.Phone,
                                       Email: request.Email,
                                       Specialization: request.Specialization,
                                       AdminId: request.AdminId);

    var result = await _mediator.Send(cmd);
    
    return result.MatchFirst(
      trainer => CreatedAtAction(actionName: nameof(GetTrainer),
                                 routeValues: new { trainerId = trainer.Id },
                                 value: new { trainer.Id, trainer.Name, trainer.Phone, trainer.Email, trainer.Specialization }),
      error => Problem(error));    
      
  }

  [HttpGet("{trainerId:guid}")]
  public async Task<IActionResult> GetTrainer(Guid trainerId)
  {
    var result = await _mediator.Send(new GetTrainerQuery(trainerId));

    return result.MatchFirst(
      trainer => Ok(new TrainerResponse(Id: trainer.Id,
                                        Name: trainer.Name,
                                        Phone: trainer.Phone,
                                        Specialization: trainer.Specialization,
                                        GymId: trainer.GymId,
                                        GymName: trainer.Gym.Name,
                                        AdminId: trainer.AdminId)),
      error => Problem(error));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllTrainers()
  {
    var result = await _mediator.Send(new ListTrainersQuery());

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse(trainers.Select(trainer => new TrainerResponse(
                                                                Id: trainer.Id,
                                                                Name: trainer.Name,
                                                                Phone: trainer.Phone,
                                                                Specialization: trainer.Specialization,
                                                                GymId: trainer.GymId,
                                                                GymName: trainer.Gym.Name,
                                                                AdminId: trainer.AdminId)))),
      error => Problem(error));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> RemoveTrainer(Guid id)
  {
    var result = await _mediator.Send(new RemoveTrainerCommand(id));

    return result.MatchFirst<IActionResult>(
      trainer => NoContent(),
      error => Problem(error));
  }
}
