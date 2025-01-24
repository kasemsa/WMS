
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.DataBase;
using WarehouseManagementSystem.DataBase.Seeders;

namespace WarehouseManagementSystem.Contract.SeedServices
{
    public class SeedService : ISeedService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeedService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WarehouseDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<WarehouseDbContext>>()))
            {
                var PermissionSeeder = new PermissionSeeder(context);
                await PermissionSeeder.Seed();
                
                var RoleSeeder = new RoleSeeder(context);
                await RoleSeeder.Seed();
                
                var RolePermissionSeeder = new RolePermissionSeeder(context);
                await RolePermissionSeeder.Seed();
            }
        }
    }
}
