using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ErrorOr;

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
        var createSubscriptionResult = await _mediator.Send(new CreateSubscriptionCommand(
                                                                    request.SubscriptionType.ToString(),
                                                                    request.AdminId));

        return createSubscriptionResult.MatchFirst(
            subscription => CreatedAtAction(actionName: nameof(GetSubscription),
                                            routeValues: new { subscriptionId = subscription.Id },
                                            value: subscription), // Pass null or the created resource            
            errors => Problem()
        );
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(subscriptionId));

        return result.MatchFirst(
          subscription => Ok(new SubscriptionResponse(subscription.Id,
                                                     Enum.Parse<SubstriptionType>(subscription.SubscriptionType))),
          error => Problem(title: error.Code, detail: error.Description,
                           statusCode: error.Type == ErrorType.NotFound ? 404 : 500)
      );        
    }    
}