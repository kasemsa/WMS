using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Price")]
        public decimal Price { get; set; } = 0;

        [JsonPropertyName("ProductImage")]
        public IFormFile? ProductImage { get; set; }
    }
}
