using Newtonsoft.Json;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class SalesInvoiceDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("TotalProductsPrice")]
        public decimal TotalProductsPrice { get; set; }

        [JsonProperty("PreviousBalance")]
        public decimal PreviousBalance { get; set; }

        [JsonProperty("InvoiceTotal")]
        public decimal InvoiceTotal { get; set; }

        [JsonProperty("DiscountValue")]
        public decimal DiscountValue { get; set; }

        [JsonProperty("DiscountType")]
        public DiscountType? DiscountType { get; set; }

        [JsonProperty("Payment")]
        public decimal Payment { get; set; }

        [JsonProperty("CurrentBalance")]
        public decimal CurrentBalance { get; set; }

        [JsonProperty("QRCodeContent")]
        public string QRCodeContent { get; set; }

        [JsonProperty("CustomerId")]
        public int CustomerId { get; set; }

        [JsonProperty("CommissaryId")]
        public int CommissaryId { get; set; }

        [JsonProperty("InvoiceItems")]
        public List<InvoiceItemDto> InvoiceItems { get; set; }
    }
}
