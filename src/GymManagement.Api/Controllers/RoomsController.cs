using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Rooms;
using GymManagement.Application.Rooms.Commands.CreateRoom;
using GymManagement.Application.Rooms.Queries.GetRoom;
using GymManagement.Application.Rooms.Queries.ListRooms;
using GymManagement.Application.Rooms.Commands.DisableRoom;
using GymManagement.Application.Rooms.Commands.UpdateRoom;

namespace GymManagement.Api.Controllers;

public class RoomsController : ApiBaseController
{
  private readonly ISender _mediator;
  public RoomsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateRoom(CreateUpdateRoomRequest request)
  {
    var cmd = new CreateRoomCommand(request.Name,
                                request.Capacity,
                                request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
        room => CreatedAtAction(actionName: nameof(GetRoom),
                                        routeValues: new { roomId = room.Id },
                                        value: new { room.Id, room.Name, room.Capacity, room.GymId }),
        error => Problem(error));
  }

  [HttpGet("{roomId:guid}")]
  public async Task<IActionResult> GetRoom(Guid roomId)
  {
    var result = await _mediator.Send(new GetRoomQuery(roomId));

    return result.MatchFirst(
      room => Ok(new RoomResponse(room.Id,
                                  room.Name,
                                  room.Capacity,
                                  room.IsAvailable,
                                  room.GymId,
                                  room.Gym.Name
                                  )),
      error => Problem(error)
  );
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllRooms()
  {
    var result = await _mediator.Send(new ListRoomsQuery());

    return result.MatchFirst(
      rooms => Ok(new ListRoomsResponse(rooms.Select(room => new RoomResponse(room.Id,
                                                                              room.Name,
                                                                              room.Capacity,
                                                                              room.IsAvailable,
                                                                              room.GymId,
                                                                              room.Gym.Name
                                                                              )))),
      error => Problem(error));
  }


  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DisableRoom(Guid id)
  {
    var result = await _mediator.Send(new DisableRoomCommand(id));

    return result.MatchFirst<IActionResult>(
      room => NoContent(),
      error => Problem(error));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateRoomm([FromRoute] Guid id, CreateUpdateRoomRequest request)
  {
    var cmd = new UpdateRoomCommand(id,
                                    request.Name,
                                    request.Capacity,
                                    request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
      room => Ok(new RoomResponse(room.Id,
                                  room.Name,
                                  room.Capacity,
                                  room.IsAvailable,
                                  room.GymId,
                                  room.Gym.Name
                                  )),
      error => Problem(error));
  }
}