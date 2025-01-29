using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Price")]
        public decimal Price { get; set; } = 0;

        [JsonPropertyName("UpdateOnImage")]
        public bool UpdateOnImage { get; set; } = false;

        [JsonPropertyName("ProductImage")]
        public IFormFile? ProductImage { get; set; }
    }
}
