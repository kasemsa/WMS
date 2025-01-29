namespace WarehouseManagementSystem.Helper
{
    public static class GlobalAttributes
    {
        public static SqlConfiguration sqlConfiguration = new SqlConfiguration();
    }
    public class SqlConfiguration
    {
        public string connectionString { get; set; }
    } 
}
