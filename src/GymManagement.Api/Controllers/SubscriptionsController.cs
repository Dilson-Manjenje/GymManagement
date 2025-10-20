using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using GymManagement.Contracts.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        var createSubscriptionResult = await _mediator.Send(new CreateSubscriptionCommand(request.SubscriptionType.ToString(),
                                                                                request.AdminId));

        return createSubscriptionResult.MatchFirst(
            //success => CreatedAtAction(nameof(GetSubscription), new { id = success }, null),
            guid => Ok(new SubscriptionResponse(createSubscriptionResult.Value,request.SubscriptionType)), 
            errors => Problem()
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscription(Guid id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionQuery(id));
        
        return Ok(subscription);
    }    
}