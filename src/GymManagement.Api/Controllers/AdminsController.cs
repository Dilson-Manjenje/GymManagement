using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Users;
using GymManagement.Application.Users.Commands.Register;
using GymManagement.Application.Users.Queries.GetAdmin;
using GymManagement.Application.Users.Queries.ListAdmins;


namespace GymManagement.Api.Controllers;

public class AdminsController : ApiBaseController
{
  private readonly ISender _mediator;
  public AdminsController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("Register")]
  public async Task<IActionResult> CreateAdmin([FromBody] RegisterRequest request, CancellationToken cancellationToken)
  {
    var cmd = new RegisterUserCommand(UserName: request.UserName,
                                      Password: request.Password);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(      
      admin => CreatedAtAction(actionName: nameof(GetAdmin),
                                 routeValues: new { adminId = admin.Id },
                                 value: new { admin.Id, admin.UserName, admin.UserId, admin.SubscriptionId }),
      error => Problem(error));
  }

  [HttpGet("{adminId:guid}")]
  public async Task<IActionResult> GetAdmin(Guid adminId)
  {
    var result = await _mediator.Send(new GetAdminQuery(adminId));

    return result.MatchFirst(
      admin => Ok(new GetAdminResponse(Id: admin.Id,
                                       UserName: admin.UserName,
                                       UserId: admin.UserId,
                                       SubscriptionId: admin.SubscriptionId)),
      error => Problem(error));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllAdmins()
  {
    var result = await _mediator.Send(new ListAdminsQuery());

    return result.MatchFirst(
      admins => Ok(new ListAdminsResponse(admins.Select(admin => new GetAdminResponse(Id: admin.Id,
                                                                                      UserId: admin.UserId,
                                                                                      UserName: admin.UserName,
                                                                                      SubscriptionId: admin.SubscriptionId)))),
      error => Problem(error));
  }
}


