using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos
{
    public class UserLoginDto
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
