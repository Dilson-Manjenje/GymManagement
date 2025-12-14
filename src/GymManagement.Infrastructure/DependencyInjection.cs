using GymManagement.Application.Common.Interfaces;
using GymManagement.Infrastructure.Members.Persistence;
using GymManagement.Infrastructure.Common.Persistence;
using GymManagement.Infrastructure.Gyms.Persistence;
using GymManagement.Infrastructure.Rooms.Persistence;
using GymManagement.Infrastructure.Subscriptions.Persistence;
using GymManagement.Infrastructure.Trainers.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<GymManagementDbContext>( options =>
        {
            options.UseSqlite("Data Source=GymManagement.db");
        });

        services.AddScoped<IUnitOfWork>( sp => sp.GetRequiredService<GymManagementDbContext>());
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<IGymsRepository, GymsRepository>();
        services.AddScoped<IRoomsRepository, RoomsRepository>();
        services.AddScoped<IMembersRepository, MembersRepository>();
        services.AddScoped<ITrainersRepository, TraneirsRepository>();
        
        services.AddScoped<IDBInitializer, DBInitializer>();                     

        return services;
    }
}
