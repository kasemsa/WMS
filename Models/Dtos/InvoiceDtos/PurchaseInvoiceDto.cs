using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class PurchaseInvoiceDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("CommissaryId")]
        public int CommissaryId { get; set; }

        [JsonProperty("InvoiceItems")]
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
