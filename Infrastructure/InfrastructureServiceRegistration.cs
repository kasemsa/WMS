using WarehouseManagementSystem.Infrastructure.JwtService;
using WarehouseManagementSystem.Infrastructure.JwtServicen.Authentication;

namespace WarehouseManagementSystem.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }
    }
}
