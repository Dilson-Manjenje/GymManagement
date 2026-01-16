using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Trainers.Commands.CreateTrainer;
using GymManagement.Application.Trainers.Queries.ListTrainers;
using GymManagement.Application.Trainers.Commands.DeleteTrainer;
using GymManagement.Application.Trainers.Queries.ListTrainersByGym;
using GymManagement.Application.Trainers.Queries.Dtos;
using GymManagement.Application.Trainers.Commands.UpdateTrainer;


namespace GymManagement.Api.Controllers;

public class TrainersController : ApiBaseController
{
  private readonly ISender _mediator;
  public TrainersController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateTrainer(TrainerRequest request)
  {
    var cmd = new CreateTrainerCommand(Name: request.Name,
                                       Phone: request.Phone,
                                       Email: request.Email,
                                       Specialization: request.Specialization,
                                       MemberId: request.MemberId);

    var result = await _mediator.Send(cmd);
    
    return result.MatchFirst(
      id => CreatedAtAction(actionName: nameof(GetTrainer),
                                 routeValues: new { trainerId = id },
                                 value: null),
      error => HandleErrors(result.Errors));    
      
  }

  [HttpGet("{trainerId:guid}")]
  public async Task<IActionResult> GetTrainer(Guid trainerId)
  {
    var result = await _mediator.Send(new GetTrainerQuery(trainerId));

    return result.MatchFirst(
      trainer => Ok(MapToResponse(trainer)),
      error => HandleErrors(result.Errors));
  }

  private TrainerResponse MapToResponse(TrainerDto trainer)
  {
    return new TrainerResponse(Id: trainer.Id,
                               Name: trainer.Name,
                               Phone: trainer.Phone,
                               Email: trainer.Email,
                               Specialization: trainer.Specialization,
                               IsActive: trainer.IsActive,
                               GymId: trainer.GymId,
                               GymName: trainer.GymName,
                               MemberId: trainer.MemberId);
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllTrainers()
  {
    var result = await _mediator.Send(new ListTrainersQuery());

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse(trainers.Select(trainer => MapToResponse(trainer)))),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update([FromRoute] Guid id, TrainerRequest request)
  {
    var cmd = new UpdateTrainerCommand(Id: id,
                                       Name: request.Name,
                                       Phone: request.Phone,
                                       Email: request.Email,
                                       Specialization: request.Specialization);
                                       
    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
      id => Ok(new { Id = id }),
      error => HandleErrors(result.Errors));
  }
  
  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> RemoveTrainer(Guid id)
  {
    var result = await _mediator.Send(new DeleteTrainerCommand(id));

    return result.MatchFirst(
      _ => NoContent(),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("List/{gymId:guid}")]
  public async Task<IActionResult> ListAllByGym([FromRoute]Guid gymId)
  {
    var result = await _mediator.Send(new ListTrainersByGymQuery(GymId: gymId));

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse( trainers.Select(trainer => MapToResponse(trainer)))),
      error => HandleErrors(result.Errors));
  }
}
