using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models.Dtos.CustomerDtos
{
    public class CreateCustomerDto
    {
        [JsonProperty("Name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("City")]
        [Required]
        public string City { get; set; } = string.Empty;

        [JsonProperty("Balance")]
        [Range(0, 10000)]
        public decimal Balance { get; set; } = 0;
    }
}
