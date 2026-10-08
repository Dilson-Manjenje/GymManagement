using GymManagement.Domain.Members;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Api.IntegrationTests.Common;

public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");

        testDatabase.InitializeDatabase();

        return testDatabase;
    }

    public void InitializeDatabase()
    {
        GymManagementDbContext context = CreateContext();

        // Seed admin user
        if (!context.Members.Any())
        {
            var adminUser = new Member(
                userName: "admin",
                gymId: null,
                userId: new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"),
                id: new Guid("7d555faf-06b9-409f-a3ba-60d2a6bfc228")
            );

            context.Members.Add(adminUser);
            context.SaveChanges();
        }
    }

    private GymManagementDbContext CreateContext()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<GymManagementDbContext>()
            .UseSqlite(Connection)
            .Options;

        var context = new GymManagementDbContext(options, null!);

        context.Database.EnsureCreated();
        return context;
    }

    public void ResetDatabase()
    {
        Connection.Close();

        InitializeDatabase();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        Connection.Close();
    }
}