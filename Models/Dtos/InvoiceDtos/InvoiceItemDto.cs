using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class InvoiceItemDto
    {
        public int Quantity { get; set; }
        public Unit Unit { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
    }
}
