using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Queries.ListGyms;

public record ListGymsQuery(): IRequest<ErrorOr<IEnumerable<Gym>?>>;
