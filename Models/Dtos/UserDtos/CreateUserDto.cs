using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos.UserDtos
{
    public class CreateUserDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("UserName")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("Password")]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("ConfirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [JsonProperty("RoleIds")]
        public List<int>? RoleIds { get; set; }
    }
}
