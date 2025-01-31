using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("Description")]
        public string? Description { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; } = 0;

        [JsonProperty("ProductImage")]
        public IFormFile? ProductImage { get; set; }
    }
}
