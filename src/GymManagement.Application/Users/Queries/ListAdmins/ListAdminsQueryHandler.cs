using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using MediatR;

namespace GymManagement.Application.Users.Queries.ListAdmins;

public class ListAdminsQueryHandler : IRequestHandler<ListAdminsQuery, ErrorOr<IEnumerable<Admin>?>>
{
    private readonly IAdminsRepository _adminsRepository;

    public ListAdminsQueryHandler(IAdminsRepository adminsRepository)
    {
        _adminsRepository = adminsRepository;
    }

    public async Task<ErrorOr<IEnumerable<Admin>?>> Handle(ListAdminsQuery query, CancellationToken cancellationToken)
    {
        var admins = await _adminsRepository.ListAsync();
    
        return admins?.ToList();
    }
}