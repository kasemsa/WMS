using Newtonsoft.Json;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models.Dtos.InvoiceDtos
{
    public class PurchaseInvoiceDto : AuditableEntity
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("CommissaryId")]
        public int CommissaryId { get; set; }

        [JsonProperty("CommissaryName")]
        public string? CommissaryName { get; set; }

        [JsonProperty("InvoiceItems")]
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
