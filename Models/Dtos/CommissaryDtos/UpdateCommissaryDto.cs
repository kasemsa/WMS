namespace WarehouseManagementSystem.Models.Dtos.CommissaryDtos
{
    public class UpdateCommissaryDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public decimal? Balance { get; set; } = 0;

    }
}