using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Gyms;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.ListGyms;
using GymManagement.Application.Gyms.Commands.DeleteGym;
using GymManagement.Application.Gyms.Commands.UpdateGym;
using GymManagement.Application.Gyms.Queries.Dtos;

namespace GymManagement.Api.Controllers;

public class GymsController : ApiBaseController
{
  private readonly ISender _mediator;
  public GymsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateGym(GymRequest request)
  {
    var cmd = new CreateGymCommand(request.Name,
                                  request.Address);

    var result = await _mediator.Send(cmd);
        
    return result.MatchFirst(
      id => CreatedAtAction(actionName: nameof(GetGym),
                              routeValues: new { gymId = id },
                              value: null),
      error => HandleErrors(result.Errors));    
  }

  [HttpGet("{gymId:guid}")]
  public async Task<IActionResult> GetGym(Guid gymId)
  {
    var result = await _mediator.Send(new GetGymQuery(gymId));

    return result.MatchFirst(
      gym => Ok(MapToResponse(gym)),
      error => HandleErrors(result.Errors));
  }

  private GymResponse MapToResponse(GymDto dto)
  {
    return new GymResponse(Id: dto.Id, Name: dto.Name, Address: dto.Address);      
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllGyms()
  {
    var result = await _mediator.Send(new ListGymsQuery());

    return result.MatchFirst(
      gyms => Ok(new ListGymsResponse(gyms.Select(gym => MapToResponse(gym)))),
      error => HandleErrors(result.Errors));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteGym(Guid id)
  {
    var result = await _mediator.Send(new DeleteGymCommand(id));

    return result.MatchFirst<IActionResult>(
      _ => NoContent(),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateGym([FromRoute] Guid id, GymRequest request)
  {
    var result = await _mediator.Send(new UpdateGymCommand(id, request.Name, request.Address));

    return result.MatchFirst(
      id => Ok(new { gymId = id }),
      error => HandleErrors(result.Errors));
  }
}
