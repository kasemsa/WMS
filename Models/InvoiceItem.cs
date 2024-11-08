using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models
{
    public class InvoiceItem : AuditableEntity
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Unit Unit { get; set; }
        public decimal Price { get; set; } = 0;
        public Product Product { get; set; } = null!;
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

    }
}
