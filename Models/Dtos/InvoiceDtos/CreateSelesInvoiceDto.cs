using Newtonsoft.Json;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class CreateSelesInvoiceDto
    {
        [JsonProperty("Payment")]
        public decimal Payment { get; set; }

        [JsonProperty("DiscountValue")]
        public decimal DiscountValue { get; set; }

        [JsonProperty("DiscountType")]
        public DiscountType? DiscountType { get; set; }

        [JsonProperty("CustomerId")]
        public int CustomerId { get; set; }

        [JsonProperty("CommissaryId")]
        public int CommissaryId { get; set; }

        [JsonProperty("InvoiceItems")]
        public required List<InvoiceItemDto> InvoiceItems { get; set; } = [];
    }
}
