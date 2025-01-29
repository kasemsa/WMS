using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos
{
    public class RolePermissionDto
    {
        [JsonProperty("PermissionIds")]
        public List<int> PermissionIds { get; set; }

        [JsonProperty("RoleId")]
        public int RoleId {  get; set; }
    }
}
