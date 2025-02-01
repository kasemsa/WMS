using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class UserRoleSeeder
    {
        private readonly WarehouseDbContext _context;

        public UserRoleSeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.UserRoles.Any())
            {
                await _context.UserRoles.AddRangeAsync(
                   new UserRole
                   {
                       // Id = 1,
                       UserId = 2,
                       RoleId = 3
                   },
                   new UserRole
                   {
                       //id = 2,
                       UserId = 1,
                       RoleId = 1
                   });
                await _context.SaveChangesAsync();
            }
        }
    }
}
