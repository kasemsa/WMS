using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class InvoiceItem : AuditableEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public Unity Unity { get; set; }
        public decimal Price { get; set; } = 0;
    }
}
