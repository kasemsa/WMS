using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Customer : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
    }
}
