using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Members;
using GymManagement.Application.Members.Commands.Register;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Members.Queries.ListMembers;


namespace GymManagement.Api.Controllers;

public class MembersController : ApiBaseController
{
  private readonly ISender _mediator;
  public MembersController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("Register")]
  public async Task<IActionResult> CreateMember([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
  {
    var cmd = new RegisterUserCommand(UserName: request.UserName,
                                      Password: request.Password,
                                      GymId: request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(      
      member => CreatedAtAction(actionName: nameof(GetMember),
                                 routeValues: new { memberId = member.Id },
                                 value: new
                                 {
                                   member.Id,
                                   member.UserName,
                                   member.UserId
                                 }),
      error => Problem(error));
  }

  [HttpGet("{memberId:guid}")]
  public async Task<IActionResult> GetMember(Guid memberId)
  {
    var result = await _mediator.Send(new GetMemberQuery(memberId));

    return result.MatchFirst(
      member => Ok(new GetMemberResponse(Id: member.Id,
                                       UserName: member.UserName,
                                       UserId: member.UserId,
                                       CurrentSubscriptionId: member.CurrentSubscription?.Id,
                                       GymId: member.Gym?.Id,
                                       GymName: member.Gym?.Name)),
      error => Problem(error));
  }

  [HttpGet("List")]
  public async Task<IActionResult> ListAllMembers()
  {
    var result = await _mediator.Send(new ListMembersQuery());

    return result.MatchFirst(
      members => Ok(new ListMembersResponse(members.Select
                                                (member => new GetMemberResponse
                                                              (Id: member.Id,
                                                                UserId: member.UserId,
                                                                UserName: member.UserName,
                                                                CurrentSubscriptionId: member.CurrentSubscription?.Id,
                                                                GymId: member.Gym?.Id,
                                                                GymName: member.Gym?.Name)))),
      error => Problem(error));
  }
}


