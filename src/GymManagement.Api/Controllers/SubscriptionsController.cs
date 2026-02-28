using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DomainSubscriptionType = GymManagement.Domain.Subscriptions.SubscriptionType;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptions;
using GymManagement.Application.Subscriptions.Commands.DeleteSubscription;
using GymManagement.Application.Subscriptions.Commands.UpdateSubscription;
using GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;
using GymManagement.Application.Subscriptions.Commands.RemoveRoomFromSubscription;
using GymManagement.Api.Mappings;
using GymManagement.Application.Subscriptions.Commands.DisableSubscription;

namespace GymManagement.Api.Controllers;

public class SubscriptionsController : ApiBaseController
{
    private readonly ISender _mediator;
    public SubscriptionsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        // TODO: Move this validation to Application, removing api from accessing the domain
        if (!DomainSubscriptionType.TryFromName(request.SubscriptionType.ToString(),
                                                out var subscriptionType))
        {
            return InvalidSubscriptionType(request.SubscriptionType.ToString());
        }

        var cmd = new CreateSubscriptionCommand(subscriptionType, request.MemberId);
        var result = await _mediator.Send(cmd);

        return result.MatchFirst(
            id => CreatedAtAction(actionName: nameof(GetSubscription),
                                            routeValues: new { subscriptionId = id },
                                            value: null // Pass null or the created resource   
                                            ),          
            error => HandleErrors(result.Errors));
    }

    private IActionResult InvalidSubscriptionType(string subscriptionType)
    {
        return Problem(statusCode: StatusCodes.Status400BadRequest,
                       detail: $"Subscription type '{subscriptionType}' is invalid.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSubscription(Guid id)
    {
        var result = await _mediator.Send(new DeleteSubscriptionCommand(id));

        return result.MatchFirst(
          _ => NoContent(),
          error => HandleErrors(result.Errors));
    }

    [HttpPut("{id:guid}/Disable")]
    public async Task<IActionResult> DisableSubscription(Guid id)
    {
        var result = await _mediator.Send(new DisableSubscriptionCommand(id));

        return result.MatchFirst(
          id => Ok(new { id = id }),
          error => HandleErrors(result.Errors));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSubscription([FromRoute] Guid id, UpdateSubscriptionRequest request)
    {
        if (!DomainSubscriptionType.TryFromName(request.SubscriptionType.ToString(),
                                                out var subscriptionType))
        {
            return InvalidSubscriptionType(request.SubscriptionType.ToString());
        }

        var result = await _mediator.Send(new UpdateSubscriptionCommand(id, subscriptionType));

        return result.MatchFirst(
          id => Ok(new { Id = id }),
          error => HandleErrors(result.Errors));
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(subscriptionId));

        return result.MatchFirst(
          subscription => Ok(ContractMappings.MapToSubscriptionResponse(subscription)),
          error => HandleErrors(result.Errors)
      );
    }

    [HttpGet("List")]
    public async Task<IActionResult> ListSubscriptions()
    {
        var result = await _mediator.Send(new ListSubscriptionsQuery());

        return result.MatchFirst(
          subscriptions => Ok(new ListSubscriptionsResponse(subscriptions.Select( subs => ContractMappings.MapToSubscriptionResponse(subs)))),
          error => HandleErrors(result.Errors)
      );

    }

    [HttpPut("{subscriptionId:guid}/rooms/")]
    public async Task<IActionResult> AddRoom([FromRoute] Guid subscriptionId, RoomSubscriptionRequest request)
    {
        var result = await _mediator.Send(new AddRoomToSubscriptionCommand(SubscriptionId: subscriptionId,
                                                                           RoomId: request.RoomId));

        return result.MatchFirst(
          id => Ok(new { id = id }),
          error => HandleErrors(result.Errors));
    }

    [HttpDelete("{subscriptionId:guid}/rooms")]
    public async Task<IActionResult> RemoveRoom([FromRoute] Guid subscriptionId, RoomSubscriptionRequest request)
    {  
        var result = await _mediator.Send(new RemoveRoomFromSubscriptionCommand(SubscriptionId: subscriptionId,
                                                                              RoomId: request.RoomId));

        return result.MatchFirst<IActionResult>(
         _ => NoContent(),
         error => HandleErrors(result.Errors));          
    }
}