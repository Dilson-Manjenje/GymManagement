using GymManagement.Application.Common.Interfaces;
using GymManagement.Infrastructure.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {        
        services.AddSingleton<ISubscriptionsRepository, SubscriptionsRepository>();
        return services;
    }
}
