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
using GymManagement.Application.Sessions.Queries.ListSessionsByGym;
using GymManagement.Application.Members.Queries.ListMembersByGym;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByGym;
using GymManagement.Application.Rooms.Queries.ListRoomsByGym;
using GymManagement.Application.Trainers.Queries.ListTrainersByGym;
using GymManagement.Api.Common;

namespace GymManagement.Api.Controllers;

public class GymsController : ApiBaseController
{
  private readonly ISender _mediator;
  public GymsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateGym(GymRequest request, CancellationToken cancellationToken = default)
  {
    var cmd = new CreateGymCommand(request.Name,
                                  request.Address);

    var result = await _mediator.Send(cmd, cancellationToken);
        
    return result.MatchFirst(
      id => CreatedAtAction(actionName: nameof(GetGym),
                              routeValues: new { id },
                              value: new ResourceIdentifier(id)),
      error => HandleErrors(result.Errors));    
  }

  [HttpGet(ApiEndPoints.Gyms.Get)]
  public async Task<IActionResult> GetGym(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new GetGymQuery(id), cancellationToken);

    return result.MatchFirst(
      gym => Ok(ContractMappings.MapToGymResponse(gym)),
      error => HandleErrors(result.Errors));
  }


  //[HttpGet("List")]
  [HttpGet(ApiEndPoints.Gyms.List)]
  public async Task<IActionResult> ListAllGyms(CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new ListGymsQuery(), cancellationToken);

    return result.MatchFirst(
      gyms => Ok(new ListGymsResponse(gyms.Select(gym => ContractMappings.MapToGymResponse(gym)))),
      error => HandleErrors(result.Errors));
  }

  [HttpDelete(ApiEndPoints.Gyms.Delete)]
  public async Task<IActionResult> DeleteGym(Guid id, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new DeleteGymCommand(id), cancellationToken);

    return result.MatchFirst<IActionResult>(
      _ => NoContent(),
      error => HandleErrors(result.Errors));
  }

  [HttpPut(ApiEndPoints.Gyms.Update)]
  public async Task<IActionResult> UpdateGym([FromRoute] Guid id, GymRequest request, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new UpdateGymCommand(id, request.Name, request.Address), cancellationToken);

    return result.MatchFirst(
      id => Ok(new ResourceIdentifier(id)),
      error => HandleErrors(result.Errors));
  }

  //[HttpGet("{gymId:guid}/Rooms")]
  [HttpGet(ApiEndPoints.Gyms.Rooms)]
  public async Task<IActionResult> ListRooms([FromRoute] Guid gymId, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new ListRoomsByGymQuery(GymId: gymId), cancellationToken);

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => ContractMappings.MapToRoomResponse(room)))),
      error => HandleErrors(result.Errors));
  }
  
  //[HttpGet("{gymId:guid}/Trainers")]
  [HttpGet(ApiEndPoints.Gyms.Trainers)]
  public async Task<IActionResult> ListTrainers([FromRoute] Guid gymId, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new ListTrainersByGymQuery(GymId: gymId), cancellationToken);

    return result.MatchFirst(
      trainers => Ok(new ListTrainersResponse(trainers.Select(trainer => ContractMappings.MapToTrainerResponse(trainer)))),
      error => HandleErrors(result.Errors));
  }


  //[HttpGet("{gymId:guid}/Sessions")]
  [HttpGet(ApiEndPoints.Gyms.Sessions)]
  public async Task<IActionResult> ListSessions(Guid gymId, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new ListSessionsByGymQuery(GymId: gymId), cancellationToken);

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }

  //[HttpGet("{gymId:guid}/Members")]
  [HttpGet(ApiEndPoints.Gyms.Members)]
  public async Task<IActionResult> ListMembers([FromRoute] Guid gymId, CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(new ListMembersByGymQuery(GymId: gymId), cancellationToken);

    return result.MatchFirst(
      members => Ok(new ListMembersResponse(members.Select(member => ContractMappings.MapToMemberResponse(member)))),
      error => HandleErrors(result.Errors));
  }


  //[HttpGet("{gymId:guid}/Subscriptions")]
  [HttpGet(ApiEndPoints.Gyms.Subscriptions)]
   public async Task<IActionResult> ListSubscriptions([FromRoute] Guid gymId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListSubscriptionsByGymQuery(GymId: gymId), cancellationToken);

        return result.MatchFirst(
          subscriptions => Ok(new ListSubscriptionsResponse(subscriptions.Select(subs => ContractMappings.MapToSubscriptionResponse(subs)))),
          error => HandleErrors(result.Errors));
    }

}
