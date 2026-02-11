using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Rooms;
using GymManagement.Application.Rooms.Commands.CreateRoom;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Rooms.Queries.ListRooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Application.Rooms.Commands.UpdateRoom;
using GymManagement.Application.Rooms.Queries.ListRoomsByGym;
using GymManagement.Application.Sessions.Queries.ListSessionsByRoom;
using GymManagement.Contracts.Sessions;
using GymManagement.Api.Mappings;

namespace GymManagement.Api.Controllers;

public class RoomsController : ApiBaseController
{
  private readonly ISender _mediator;
  public RoomsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateRoom(RoomRequest request)
  {
    var cmd = new CreateRoomCommand(request.Name,
                                request.Capacity,
                                request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
        id => CreatedAtAction(actionName: nameof(GetRoom),
                                        routeValues: new { roomId = id },
                                        value: null ),
        error => HandleErrors(result.Errors));
  }

  [HttpGet("{roomId:guid}")]
  public async Task<IActionResult> GetRoom(Guid roomId)
  {
    var result = await _mediator.Send(new GetRoomQuery(roomId));

    return result.MatchFirst(
      room => Ok(ContractMappings.MapToRoomResponse(room)),
      error => HandleErrors(result.Errors));
  }


  [HttpGet("List")]
  public async Task<IActionResult> ListAllRooms()
  {
    var result = await _mediator.Send(new ListRoomsQuery());

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => ContractMappings.MapToRoomResponse(room)))),
      error => HandleErrors(result.Errors));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DisableRoom(Guid id)
  {
    var result = await _mediator.Send(new DisableRoomCommand(id));

    return result.MatchFirst<IActionResult>(
      _ => NoContent(),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateRoomm([FromRoute] Guid id, RoomRequest request)
  {
    var cmd = new UpdateRoomCommand(id,
                                    request.Name,
                                    request.Capacity,
                                    request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
      id => Ok(new { Id = id }),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("{roomId:guid}/Sessions")]
  public async Task<IActionResult> ListSessions(Guid roomId)
  {
    var result = await _mediator.Send(new ListSessionsByRoomQuery(RoomId: roomId));

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }
}