using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using MediatR;

namespace GymManagement.Application.Users.Queries.GetAdmin;

public class GetAdminQueryHandler : IRequestHandler<GetAdminQuery, ErrorOr<Admin>>
{
    private readonly IAdminsRepository _adminsRepository;

    public GetAdminQueryHandler(IAdminsRepository adminsRepository)
    {
        _adminsRepository = adminsRepository;
    }

    public async Task<ErrorOr<Admin>> Handle(GetAdminQuery query, CancellationToken cancellationToken)
    {
        var admin = await _adminsRepository.GetByIdAsync(query.AdminId);

        return (admin is null)
           ? AdminErrors.UserNotFound(query.AdminId)
           : admin;
    }
}