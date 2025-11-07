using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Gyms;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.ListGyms;
using GymManagement.Application.Gyms.Commands.DeleteGym;
using GymManagement.Application.Gyms.Commands.UpdateGym;

namespace GymManagement.Api.Controllers;

public class GymsController : ApiBaseController
{
  private readonly ISender _mediator;
  public GymsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateGym(CreateUpdateGymRequest request)
  {
    var cmd = new CreateGymCommand(request.Name,
                                  request.Address);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
        gym => CreatedAtAction(actionName: nameof(GetGym),
                              routeValues: new { gymId = gym.Id },
                              value: new { gym.Id, gym.Name, gym.Address }),
      error => Problem(error));    
  }

  [HttpGet("{gymId:guid}")]
  public async Task<IActionResult> GetGym(Guid gymId)
  {
    var result = await _mediator.Send(new GetGymQuery(gymId));

    return result.MatchFirst(
      gym => Ok(new GymResponse(gym.Id, gym.Name, gym.Address)),
      error => Problem(error));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllGyms()
  {
    var result = await _mediator.Send(new ListGymsQuery());

    return result.MatchFirst(
      gyms => Ok(new ListGymsResponse(gyms.Select(gym => new GymResponse(gym.Id, gym.Name, gym.Address)))),
      error => Problem(error));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteGym(Guid id)
  {
    var result = await _mediator.Send(new DeleteGymCommand(id));

    return result.MatchFirst<IActionResult>(
      gym => NoContent(),
      error => Problem(error));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateGym([FromRoute] Guid id, CreateUpdateGymRequest request)
  {
    var result = await _mediator.Send(new UpdateGymCommand(id, request.Name, request.Address));

    return result.MatchFirst(
      gym => Ok(new GymResponse(gym.Id, gym.Name, gym.Address)),
      error => Problem(error));
  }
}
