using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddMediatR(options =>
        {
            //options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        return services;
                
    }
}
