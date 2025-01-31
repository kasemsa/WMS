using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.CommissaryDtos
{
    public class UpdateCommissaryDto
    {
        [JsonProperty("Name")]
        public string? Name { get; set; } = string.Empty;

        [JsonProperty("PhoneNumber")]
        public string? PhoneNumber { get; set; } = string.Empty;

    }
}