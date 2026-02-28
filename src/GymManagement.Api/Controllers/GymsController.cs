using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Api.Mappings;
using GymManagement.Contracts.Gyms;
using GymManagement.Contracts.Sessions;
using GymManagement.Contracts.Members;
using GymManagement.Contracts.Subscriptions;
using GymManagement.Contracts.Rooms;
using GymManagement.Contracts.Trainers;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Application.Gyms.Queries.GetGym;
using GymManagement.Application.Gyms.Queries.ListGyms;
using GymManagement.Application.Gyms.Commands.DeleteGym;
using GymManagement.Application.Gyms.Commands.UpdateGym;
using GymManagement.Application.Gyms.Queries.Dtos;
using GymManagement.Application.Sessions.Queries.ListSessionsByGym;
using GymManagement.Application.Members.Queries.ListMembersByGym;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByGym;
using GymManagement.Application.Rooms.Queries.ListRoomsByGym;
using GymManagement.Application.Trainers.Queries.ListTrainersByGym;

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
                              routeValues: new { id = id },
                              value: null),
      error => HandleErrors(result.Errors));    
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetGym(Guid id)
  {
    var result = await _mediator.Send(new GetGymQuery(id));

    return result.MatchFirst(
      gym => Ok(ContractMappings.MapToGymResponse(gym)),
      error => HandleErrors(result.Errors));
  }


  [HttpGet("List")]
  public async Task<IActionResult> ListAllGyms()
  {
    var result = await _mediator.Send(new ListGymsQuery());

    return result.MatchFirst(
      gyms => Ok(new ListGymsResponse(gyms.Select(gym => ContractMappings.MapToGymResponse(gym)))),
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

  [HttpGet("{gymId:guid}/Rooms")]
  public async Task<IActionResult> ListRooms([FromRoute] Guid gymId)
  {
    var result = await _mediator.Send(new ListRoomsByGymQuery(GymId: gymId));

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => ContractMappings.MapToRoomResponse(room)))),
      error => HandleErrors(result.Errors));
  }
  
  [HttpGet("{gymId:guid}/Trainers")]
  public async Task<IActionResult> ListTrainers([FromRoute] Guid gymId)
  {
    var result = await _mediator.Send(new ListTrainersByGymQuery(GymId: gymId));

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse(trainers.Select(trainer => ContractMappings.MapToTrainerResponse(trainer)))),
      error => HandleErrors(result.Errors));
  }


  [HttpGet("{gymId:guid}/Sessions")]
  public async Task<IActionResult> ListSessions(Guid gymId)
  {
    var result = await _mediator.Send(new ListSessionsByGymQuery(GymId: gymId));

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("{gymId:guid}/Members")]
  public async Task<IActionResult> ListMembers([FromRoute] Guid gymId)
  {
    var result = await _mediator.Send(new ListMembersByGymQuery(GymId: gymId));

    return result.MatchFirst(
      members => Ok(new ListMembersResponse(members.Select(member => ContractMappings.MapToMemberResponse(member)))),
      error => HandleErrors(result.Errors));
  }

  
    [HttpGet("{gymId:guid}/Subscriptions")]
    public async Task<IActionResult> ListSubscriptions([FromRoute] Guid gymId)
    {
        var result = await _mediator.Send(new ListSubscriptionsByGymQuery(GymId: gymId));

        return result.MatchFirst(
          subscriptions => Ok(new ListSubscriptionsResponse(subscriptions.Select(subs => ContractMappings.MapToSubscriptionResponse(subs)))),
          error => HandleErrors(result.Errors));
    }

}
