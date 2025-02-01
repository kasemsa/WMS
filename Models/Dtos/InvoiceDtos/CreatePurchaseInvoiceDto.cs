using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class CreatePurchaseInvoiceDto
    {
        [JsonProperty("CommissaryId")]
        public int CommissaryId {  get; set; }

        [JsonProperty("InvoiceItems")]
        public List<PurchaseInvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
