namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class CreatePurchaseInvoiceDto
    {
        public int CommissaryId { get; set; }
        public List<PurchaseInvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
