namespace WarehouseManagementSystem.Models.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public IFormFile? ProductImage { get; set; } 
    }
}
