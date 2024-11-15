using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class SalesInvoiceDto
    {
        public int Id { get; set; }
        public decimal TotalProductsPrice { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal InvoiceTotal { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal Payment { get; set; }
        public decimal CurrentBalance { get; set; }
        public string QRCodeContent { get; set; }
        public int CustomerId { get; set; }
        public int CommissaryId { get; set; }
        public List<InvoiceItemDto> InvoiceItems { get; set; }
    }
}
