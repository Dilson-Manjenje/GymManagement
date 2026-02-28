using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Sessions;
using GymManagement.Application.Sessions.Commands.CreateSession;
using GymManagement.Application.Sessions.Queries.GetSession;
using GymManagement.Application.Sessions.Queries.ListSessions;
using GymManagement.Application.Sessions.Commands.UpdateSession;
using GymManagement.Application.Sessions.Commands.DeleteSession;
using GymManagement.Application.Sessions.Queries.ListUpComingSessions;
using GymManagement.Api.Mappings;
using GymManagement.Application.Sessions.Commands.CancelSession;
using GymManagement.Application.Sessions.Commands.FinalizeSession;
using GymManagement.Application.Bookings.Queries.ListBookingsBySession;
using GymManagement.Contracts.Bookings;


namespace GymManagement.Api.Controllers;
public class SessionsController : ApiBaseController
{
  private readonly ISender _mediator;
  public SessionsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateSession(CreateSessionRequest request)
  {
    var cmd = new CreateSessionCommand(RoomId: request.RoomId,
                                       TrainerId: request.TrainerId,
                                       Title: request.Title,
                                       StartDate: request.StartDate,
                                       EndDate: request.EndDate);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
        id => CreatedAtAction(actionName: nameof(GetSession),
                                   routeValues: new { id = id },
                                   value: null),
        error => HandleErrors(result.Errors));
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetSession(Guid id)
  {
    var result = await _mediator.Send(new GetSessionQuery(id));

    return result.MatchFirst(
      session => Ok(ContractMappings.MapToSessionResponse(session)),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllSessions()
  {
    var result = await _mediator.Send(new ListSessionsQuery());

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("UpCommingSessions/{gymId:guid}")]
  public async Task<IActionResult> ListUpCommingSessions(Guid gymId)
  {
    var result = await _mediator.Send(new ListUpComingSessionsQuery(GymId: gymId));

    return result.MatchFirst(
      sessions => Ok(new ListSessionsResponse(sessions.Select(session => ContractMappings.MapToSessionResponse(session)))),
      error => HandleErrors(result.Errors));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteSession(Guid id)
  {
    var result = await _mediator.Send(new DeleteSessionCommand(id));

    return result.MatchFirst<IActionResult>(
      session => NoContent(),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateSession([FromRoute] Guid id, UpdateSessionRequest request)
  {
    var cmd = new UpdateSessionCommand(Id: id,
                                       RoomId: request.RoomId,
                                       TrainerId: request.TrainerId,
                                       Title: request.Title,
                                       StartDate: request.StartDate,
                                       EndDate: request.EndDate);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
      id => Ok(new { Id = id }),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("{sessionId:guid}/Bookings")]
  public async Task<IActionResult> ListActiveBookings([FromRoute] Guid sessionId)
  {
    var result = await _mediator.Send(new ListBookingsBySessionQuery(SessionId: sessionId));

    return result.MatchFirst(
      bookings => Ok(new ListBookingResponse(bookings.Select(booking => ContractMappings.MapToBookingResponse(booking)))),
      error => HandleErrors(result.Errors));
  }
  
  [HttpPut("{sessionId:guid}/Cancel")]
  public async Task<IActionResult> Cancel([FromRoute] Guid sessionId)
  {
    var result = await _mediator.Send(new CancelSessionCommand(SessionId: sessionId));

    return result.MatchFirst(
      id => Ok(new { id = id }),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{sessionId:guid}/Finalize")]
  public async Task<IActionResult> Finalize([FromRoute] Guid sessionId)
  {
    var result = await _mediator.Send(new FinalizeSessionCommand(SessionId: sessionId));

    return result.MatchFirst<IActionResult>(
     id => Ok(new { id = id }),
     error => HandleErrors(result.Errors));
  }
  
}