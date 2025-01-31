using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementSystem.Models.Dtos.CommissaryDtos
{
    public class CreateCommissaryDto
    {
        [JsonProperty("Name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("UserName")]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("Password")]
        [Required]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("ConfirmPassword")]
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("RoleIds")]
        public List<int>? RoleIds { get; set; }
    }
}