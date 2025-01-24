using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Contract.FileServices;
using WarehouseManagementSystem.Contract.SeedServices;
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
            services.AddScoped<IFileService, FileService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<ISeedService, SeedService>();
            //services.AddTransient<AuthenticationMiddleware>();

            return services;
        }
    }
}
