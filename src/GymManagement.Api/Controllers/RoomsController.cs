using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Rooms;
using GymManagement.Application.Rooms.Commands.CreateRoom;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Rooms.Queries.ListRooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Application.Rooms.Commands.UpdateRoom;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Rooms.Queries.ListRoomsByGym;

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
      room => Ok(MapToResponse(room)),
      error => HandleErrors(result.Errors));
  }

  private RoomResponse MapToResponse(RoomDetailsDto room)
  {
      return new RoomResponse(Id: room.Id,
                              Name: room.Name,
                              Capacity: room.Capacity,
                              IsAvailable: room.IsAvailable,
                              GymId: room.GymId,
                              GymName: room.GymName);
  }


  [HttpGet("List")]
  public async Task<IActionResult> ListAllRooms()
  {
    var result = await _mediator.Send(new ListRoomsQuery());

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => MapToResponse(room)))),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("List/{gymId:guid}")]
  public async Task<IActionResult> ListAllByGym([FromRoute]Guid gymId)
  {
    var result = await _mediator.Send(new ListRoomsByGymQuery(GymId: gymId));

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => MapToResponse(room)))),
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
}