using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using MediatR;

namespace GymManagement.Application.Members.Queries.ListMembers;

public class ListMembersQueryHandler : IRequestHandler<ListMembersQuery, ErrorOr<IEnumerable<Member>?>>
{
    private readonly IMembersRepository _membersRepository;

    public ListMembersQueryHandler(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<IEnumerable<Member>?>> Handle(ListMembersQuery query, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.ListAsync();
    
        return members?.ToList();
    }
}