using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagement.Infrastructure.Common.Persistence
{
    public class DBInitializer : IDBInitializer
    {
        private readonly GymManagementDbContext _db;
        
        public DBInitializer(GymManagementDbContext db)
        {
            _db = db;
        }


        public async Task InitializeAsync()    
        {
            // Add pending migration if exists
            if (_db.Database.GetPendingMigrations().Count() == 0)
            {
                _db.Database.Migrate();
            }

            //Exit if role already exists
            //if (_db.Roles.Any(r => r.Name == Helpers.Roles.Admin)) return;

            if (!_db.Members.Any())
            {
                //Create Admin user
                var adminUser = new Member(
                    userName: "admin",
                    gymId: null,
                    userId: new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"),
                    id: new Guid("7d555faf-06b9-409f-a3ba-60d2a6bfc228")
                );

                await _db.Members.AddAsync(adminUser);
            }
            
            await _db.SaveChangesAsync();
        }
    }
}
