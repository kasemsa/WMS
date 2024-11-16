using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class PurchaseInvoiceItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Unit Unit { get; set; }
    }
}
