using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class CreatePurchaseInvoiceDto
    {
        [JsonProperty("InvoiceItems")]
        public List<PurchaseInvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
