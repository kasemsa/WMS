using Newtonsoft.Json;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class PurchaseInvoiceItemDto
    {
        [JsonProperty("ProductId")]
        public int ProductId { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonProperty("Unit")]
        public Unit Unit { get; set; }
    }
}
