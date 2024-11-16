namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class PurchaseInvoiceDto
    {
        public int Id { get; set; }
        public decimal InvoiceTotal { get; set; }
        public int CommissaryId { get; set; }
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
