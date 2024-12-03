using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.DataBase;

namespace WarehouseManagementSystem.Contract
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Default Database..
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseSqlServer(connectionString: configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            //services.AddTransient<AuthenticationMiddleware>();

            return services;
        }
    }
}
