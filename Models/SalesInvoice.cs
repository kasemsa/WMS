using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models
{
    public class SalesInvoice : AuditableEntity
    {
        public int Id { get; set; }
        public decimal TotalProductsPrice;
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal Payment { get; set; }
        public decimal InvoiceTotal { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
        public bool Refunded { get; set; } = false;
        public Customer Customer { get; set; } = null!;
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        public Commissary Commissary { get; set; } = null!;
        [ForeignKey(nameof(Commissary))]
        public int CommissaryId { get; set; }

        public required List<InvoiceItem> InvoiceItems { get; set; } = [];
    }
}
