using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class RolePermissionSeeder
    {
        private readonly WarehouseDbContext _context;

        public RolePermissionSeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.RolePermissions.Any())
            {
                await _context.RolePermissions.AddRangeAsync(
                    //----- Admin Permission----
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 1,
                         RoleId = 1,
                     },
                     //----- Commissary Permission------
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 7,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 8,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 9,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 10,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 11,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 15,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 16,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 22,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 23,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 24,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 25,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 26,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 27,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 28,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 29,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 30,
                         RoleId = 3,
                     },
                     new RolePermission()
                     {
                         //Id = 1,
                         PermissionId = 31,
                         RoleId = 3,
                     });
                await _context.SaveChangesAsync();
            }
        }
    }
}
