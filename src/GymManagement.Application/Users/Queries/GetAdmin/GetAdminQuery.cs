using ErrorOr;
using GymManagement.Domain.Admins;
using MediatR;

namespace GymManagement.Application.Users.Queries.GetAdmin;

public sealed record GetAdminQuery(Guid AdminId): IRequest<ErrorOr<Admin>>;
