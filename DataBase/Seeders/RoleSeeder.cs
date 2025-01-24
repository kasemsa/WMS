using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class RoleSeeder
    {
        private readonly WarehouseDbContext _context;

        public RoleSeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.Roles.Any())
            {
                await _context.Roles.AddRangeAsync(
                   new Role
                   {
                       // Id = 1,
                       RoleName = "Admin"
                   },
                   new Role
                   {
                       // Id = 2,
                       RoleName = "User"
                   },
                   new Role
                   {
                       // Id = 3,
                       RoleName = "Commissary"
                   });
                await _context.SaveChangesAsync();
            }
        }
    }
}
