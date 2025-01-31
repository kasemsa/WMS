using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WarehouseManagementSystem.Models.Dtos
{
    public class UserLoginDto
    {
        [JsonProperty("UserName")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
