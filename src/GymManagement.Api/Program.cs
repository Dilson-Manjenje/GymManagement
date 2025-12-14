using GymManagement.Application;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();


builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();
{
    app.UseExceptionHandler();
    
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        await dbInitializer.InitializeAsync();
    }

    app.Run();    
}

