using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models.Dtos.CommissaryDtos
{
    public class CreateCommissaryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int>? RoleIds { get; set; }
    }
}