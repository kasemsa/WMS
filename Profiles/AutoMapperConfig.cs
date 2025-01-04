namespace WarehouseManagementSystem.Profiles
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
        }
    }
}
