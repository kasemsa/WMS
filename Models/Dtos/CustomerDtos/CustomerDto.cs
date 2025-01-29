using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.CustomerDtos
{
    public class CustomerDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("City")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("Balance")]
        public decimal Balance { get; set; }

        [JsonProperty("SalesInvoices")]
        public List<SalesInvoice> SalesInvoices { get; set; } = null!;
    }
}
