using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class RefundItemDto
    {
        [JsonProperty("CustomerId")]
        public int CustomerId { get; set; }

        [JsonProperty("CommissaryId")]
        public int CommissaryId { get; set; }

        [JsonProperty("InvoiceItems")]
        public required List<InvoiceItemDto> InvoiceItems { get; set; } = [];
    }

}
