using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ErrorOr;
using DomainSubscriptionType = GymManagement.Domain.Subscriptions.SubscriptionType;
using GymManagement.Application.Subscriptions.Queries.GetAllSubscriptions;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
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
            //return BadRequest($"Invalid subscription type: {request.SubscriptionType}");
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Subscription type '{request.SubscriptionType}' is invalid.");
        }

        var cmd = new CreateSubscriptionCommand(subscriptionType, request.AdminId);
        var result = await _mediator.Send(cmd);
        
        return result.MatchFirst(
            subscription => CreatedAtAction(actionName: nameof(GetSubscription),
                                            routeValues: new { subscriptionId = subscription.Id },
                                            value: new { subscription.Id, subscriptionType = subscription.SubscriptionType.Name }), // Pass null or the created resource            
            errors => Problem()
        );
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(subscriptionId));

        return result.MatchFirst(
          subscription => Ok(new SubscriptionResponse(subscription.Id,
                                                     Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name))),
          error => Problem(title: error.Code, detail: error.Description,
                           statusCode: error.Type == ErrorType.NotFound ? 404 : 500)
      );
    }    
    
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        var result = await _mediator.Send(new GetAllSubscriptionsQuery());
        
        return result.MatchFirst(
          subscriptions => Ok(new SubscriptionsListResponse(subscriptions
                                                                .Select(subscription => new SubscriptionResponse(
                                                                                        subscription.Id,
                                                                                        Enum.Parse<SubstriptionType>(subscription.SubscriptionType.Name))))),
          error => Problem(title: error.Code, detail: error.Description,
                           statusCode: error.Type == ErrorType.NotFound ? 404 : 500)
      );
    }    
}