using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class CreateSelesInvoiceDto
    {
        public decimal Payment { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
        public int CustomerId { get; set; }
        public int CommissaryId { get; set; }
        public required List<InvoiceItemDto> InvoiceItems { get; set; } = [];
    }
}
