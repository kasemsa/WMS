using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class ProductDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("Description")]
        public string? Description { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; } = 0;

        [JsonProperty("Image")]
        public string? Image { get; set; }
    }
}
