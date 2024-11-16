namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class RefundItemDto
    {
        public int CustomerId { get; set; }
        public int CommissaryId { get; set; }
        public required List<InvoiceItemDto> InvoiceItems { get; set; } = [];
    }

}
