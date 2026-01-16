using Microsoft.AspNetCore.Mvc;
using MediatR;
using GymManagement.Contracts.Members;
using GymManagement.Application.Members.Commands.CreateMember;
using GymManagement.Application.Members.Queries.GetMember;
using GymManagement.Application.Members.Queries.ListMembers;
using GymManagement.Application.Members.Queries.ListMembersByGym;
using GymManagement.Application.Members.Commands.UpdateMember;
using GymManagement.Application.Members.Commands.DeleteMember;
using GymManagement.Application.Members.Queries.Dtos;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByMember;
using GymManagement.Contracts.Subscriptions;
using GymManagement.Api.Mappings;


namespace GymManagement.Api.Controllers;

public class MembersController : ApiBaseController
{
  private readonly ISender _mediator;
  public MembersController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("Register")]
  public async Task<IActionResult> CreateMember([FromBody] CreateMemberRequest request, CancellationToken cancellationToken)
  {
    var cmd = new CreateMemberCommand(UserName: request.UserName,
                                      Password: request.Password,
                                      GymId: request.GymId);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(      
      id => CreatedAtAction(actionName: nameof(GetMember),
                                 routeValues: new { memberId = id },
                                 value: null),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("{memberId:guid}")]
  public async Task<IActionResult> GetMember(Guid memberId)
  {
    var result = await _mediator.Send(new GetMemberQuery(memberId));

    return result.MatchFirst(
      member => Ok(ContractMappings.MapToMemberResponse(member)),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("{memberId:guid}/Subscriptions")]
  public async Task<IActionResult> GetMemberSubscriptions(Guid memberId)
  {
    var result = await _mediator.Send(new ListSubscriptionsByMemberQuery(MemberId: memberId));

    return result.MatchFirst(
      subscriptions => Ok(new ListSubscriptionsResponse(subscriptions.Select(subs => ContractMappings.MapToSubscriptionResponse(subs)))),
      error => HandleErrors(result.Errors));
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update([FromRoute] Guid id, UpdateMemberRequest request)
  {
    var cmd = new UpdateMemberCommand(Id: id,
                                      UserName: request.UserName,
                                      Password: request.Password);

    var result = await _mediator.Send(cmd);

    return result.MatchFirst(
      id => Ok(new { Id = id }),
      error => HandleErrors(result.Errors));
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteMember(Guid id)
  {
    var result = await _mediator.Send(new DeleteMemberCommand(id));

    return result.MatchFirst<IActionResult>(
      _ => NoContent(),
      error => HandleErrors(result.Errors));
  }
  
  [HttpGet("List")]
  public async Task<IActionResult> ListAllMembers()
  {
    var result = await _mediator.Send(new ListMembersQuery());

    return result.MatchFirst(
      members => Ok(new ListMembersResponse(members.Select(member => ContractMappings.MapToMemberResponse(member)))),
      error => HandleErrors(result.Errors));
  }

  [HttpGet("List/{gymId:guid}")]
  public async Task<IActionResult> ListAllByGym([FromRoute]Guid gymId)
  {
    var result = await _mediator.Send(new ListMembersByGymQuery(GymId: gymId));

    return result.MatchFirst(
      members => Ok(new ListMembersResponse(members.Select(member => ContractMappings.MapToMemberResponse(member)))),
      error => HandleErrors(result.Errors));
  }
}


