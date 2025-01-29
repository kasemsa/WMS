using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos.UserDtos
{
    public class UserDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonProperty("Email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("UserName")]
        public string UserName { get; set; } = string.Empty;
        [JsonProperty("Roles")]
        public List<Role> Roles { get; set; } = null!;
        [JsonProperty("Permissions")]
        public List<Permission> Permissions { get; set; } = null!;
    }
}
