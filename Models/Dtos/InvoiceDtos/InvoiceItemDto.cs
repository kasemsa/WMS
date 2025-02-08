using Newtonsoft.Json;
using WarehouseManagementSystem.Models.Constants;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class InvoiceItemDto
    {
        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonProperty("Unit")]
        public Unit Unit { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("ProductId")]
        public int ProductId { get; set; }

        [JsonProperty("ProductName")]
        public string? ProductName { get; set; }
    }
}
