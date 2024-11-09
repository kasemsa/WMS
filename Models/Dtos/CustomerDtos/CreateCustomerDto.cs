using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models.Dtos.CustomerDtos
{
    public class CreateCustomerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Range(0, 10000)]
        public decimal Balance { get; set; } = 0;
    }
}
