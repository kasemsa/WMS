namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public bool UpdateOnImage { get; set; } = false;
        public IFormFile? ProductImage { get; set; }
    }
}
