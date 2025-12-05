using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DomainSubscriptionType = GymManagement.Domain.Subscriptions.SubscriptionType;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptions;
using GymManagement.Application.Subscriptions.Commands.DeleteSubscription;
using GymManagement.Application.Subscriptions.Commands.UpdateSubscription;

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
        if (!DomainSubscriptionType.TryFromName(request.SubscriptionType.ToString(),
                                                out var subscriptionType))
        {
            return InvalidSubscriptionType(request.SubscriptionType.ToString());
        }

        var cmd = new CreateSubscriptionCommand(subscriptionType, request.AdminId);
        var result = await _mediator.Send(cmd);
        
        return result.MatchFirst(
            subscription => CreatedAtAction(actionName: nameof(GetSubscription),
                                            routeValues: new { subscriptionId = subscription.Id },
                                            value: new SubscriptionResponse(Id: subscription.Id,
                                                       SubscriptionType: Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name),
                                                       Price: subscription.Price,
                                                       MaxRooms: subscription.GetMaxRooms(),
                                                       MaxDailySessions: subscription.MaxDailySessions,
                                                       AdminId: subscription.Admin.Id,
                                                       UserName: subscription.Admin.UserName)
                                            ), // Pass null or the created resource            
            error => Problem(error));
    }

    private IActionResult InvalidSubscriptionType(string subscriptionType)
    {
        return Problem(statusCode: StatusCodes.Status400BadRequest,
                       detail: $"Subscription type '{subscriptionType}' is invalid.");
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(subscriptionId));

        return result.MatchFirst(
          subscription => Ok(new SubscriptionResponse(Id: subscription.Id,
                                                       SubscriptionType: Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name),
                                                       Price: subscription.Price,
                                                       MaxRooms: subscription.GetMaxRooms(),
                                                       MaxDailySessions: subscription.MaxDailySessions,
                                                       AdminId: subscription.Admin.Id,
                                                       UserName: subscription.Admin.UserName
                                                    )),
          error => Problem(error)
      );
    }

    [HttpGet("List")]
    public async Task<IActionResult> ListSubscriptions()
    {
        var result = await _mediator.Send(new ListSubscriptionsQuery());

        return result.MatchFirst(
          subscriptions => Ok(new SubscriptionsListResponse(subscriptions
                                                                .Select(subscription => new SubscriptionResponse(Id: subscription.Id,
                                                                    SubscriptionType: Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name),
                                                                    Price: subscription.Price,
                                                                    MaxRooms: subscription.GetMaxRooms(),
                                                                    MaxDailySessions: subscription.MaxDailySessions,
                                                                    AdminId: subscription.Admin.Id,
                                                                    UserName: subscription.Admin.UserName)))),
          error => Problem(error)
      );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSubscription(Guid id)
    {
        var result = await _mediator.Send(new DeleteSubscriptionCommand(id));

        return result.MatchFirst<IActionResult>(
          subscription => NoContent(),
          error => Problem(error));
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
          subscription => Ok(new SubscriptionResponse(Id: subscription.Id,
                                                      SubscriptionType: Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name),
                                                       Price: subscription.Price,
                                                       MaxRooms: subscription.GetMaxRooms(),
                                                       MaxDailySessions: subscription.MaxDailySessions,
                                                       AdminId: subscription.Admin.Id,
                                                       UserName: subscription.Admin.UserName             
                                                       )),
          error => Problem(error));
    }
}