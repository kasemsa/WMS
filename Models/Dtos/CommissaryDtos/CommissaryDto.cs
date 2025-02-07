using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.CommissaryDtos
{
    public class CommissaryDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string? Name { get; set; }

        [JsonProperty("Email")]
        public string? Email { get; set; }

        [JsonProperty("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}