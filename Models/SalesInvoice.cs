using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class SalesInvoice : AuditableEntity
    {
        public int Id { get; set; }

        public required ICollection<InvoiceItem> InvoiceItems { get; set; }
        public int TotalQuantity { get; set; }

        public decimal TotalProductsPrice;

        public decimal PreviousBalance { get; set; }

        public decimal InvoiceTotal { get; set; }

        public decimal DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal DiscountVaule { get; set; }

        public decimal Payment { get; set; }
        public string QRCodeContent { get; set; }
        public decimal CurrentBalance { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CommissaryId { get; set; }
        public Commissary Commissary { get; set; }
    }
}
