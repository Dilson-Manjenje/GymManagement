using ErrorOr;
using GymManagement.Domain.Admins;
using MediatR;

namespace GymManagement.Application.Users.Queries.ListAdmins;

public record ListAdminsQuery(): IRequest<ErrorOr<IEnumerable<Admin>?>>;
