using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Api.Mappings;
using GymManagement.Contracts.Bookings;
using GymManagement.Application.Bookings.Commands.CreateBooking;
using GymManagement.Application.Bookings.Queries.GetBooking;
using GymManagement.Application.Bookings.Queries.ListBookings;
using GymManagement.Application.Bookings.Commands.CancelBooking;
using GymManagement.Application.Bookings.Commands.FinalizeBooking;


namespace GymManagement.Api.Controllers;

public class BookingsController : ApiBaseController
{
  private readonly ISender _mediator;
  public BookingsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateBooking(CreateBookingRequest request)
  {
    var cmd = new CreateBookingCommand(SessionId: request.SessionId,
                                       MemberId: request.MemberId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
        id => CreatedAtAction(actionName: nameof(GetBooking),
                                   routeValues: new { id = id },
                                   value: null),
        error => HandleErrors(result.Errors));
  }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var result = await _mediator.Send(new GetBookingQuery(BookingId: id));

        return result.MatchFirst(
          booking => Ok(ContractMappings.MapToBookingResponse(booking)),
          error => HandleErrors(result.Errors));
    }

  [HttpGet("List")]
  public async Task<IActionResult> ListAll()
  {
    var result = await _mediator.Send(new ListBookingsQuery());

    return result.MatchFirst(
      bookings => Ok(new ListBookingResponse(bookings.Select(booking => ContractMappings.MapToBookingResponse(booking)))),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{bookingId:guid}/Cancel")]
  public async Task<IActionResult> Cancel([FromRoute] Guid bookingId)
  {
    var result = await _mediator.Send(new CancelBookingCommand(BookingId: bookingId));

    return result.MatchFirst(
      id => Ok(new { id = id }),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{bookingId:guid}/Finalize")]
  public async Task<IActionResult> Finalize([FromRoute] Guid bookingId)
  {
    var result = await _mediator.Send(new FinalizeBookingCommand(BookingId: bookingId));

    return result.MatchFirst<IActionResult>(
     id => Ok(new { id = id }),
     error => HandleErrors(result.Errors));
  }
  
}
