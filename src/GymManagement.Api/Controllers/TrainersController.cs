using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Trainers;
using GymManagement.Application.Trainers.Queries.GetTrainer;
using GymManagement.Application.Trainers.Commands.CreateTrainer;
using GymManagement.Application.Trainers.Queries.ListTrainers;
using GymManagement.Application.Trainers.Commands.DeleteTrainer;
using GymManagement.Application.Trainers.Commands.UpdateTrainer;
using GymManagement.Application.Sessions.Queries.ListSessionsByTrainer;
using GymManagement.Contracts.Sessions;
using GymManagement.Api.Mappings;


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
      trainer => Ok(ContractMappings.MapToTrainerResponse(trainer)),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllTrainers()
  {
    var result = await _mediator.Send(new ListTrainersQuery());

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse(trainers.Select(trainer => ContractMappings.MapToTrainerResponse(trainer)))),
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

  [HttpGet("{trainerId:guid}/Sessions")]
  public async Task<IActionResult> ListSessions(Guid trainerId)
  {
    var result = await _mediator.Send(new ListSessionsByTrainerQuery(TrainerId: trainerId));

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }
  
}
