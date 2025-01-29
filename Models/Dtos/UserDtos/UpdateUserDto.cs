using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("UserName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("RoleIds")]
        public List<int>? RoleIds { get; set; }
    }
}
