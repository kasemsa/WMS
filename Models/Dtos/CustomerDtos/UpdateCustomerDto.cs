using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.CustomerDtos
{
    public class UpdateCustomerDto
    {
        [JsonProperty("Name")]
        public string? Name { get; set; }

        [JsonProperty("City")]
        public string? City { get; set; }

        [JsonProperty("Balance")]
        public decimal? Balance { get; set; }
    }
}
