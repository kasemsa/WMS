using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos
{
    public class AsignPermissionToUserRequset
    {
        [JsonProperty("PermissionId")]
        public List<int> PermissionId { get; set; }

        [JsonProperty("UserId")]
        public int UserId { get; set; }
    }
}
