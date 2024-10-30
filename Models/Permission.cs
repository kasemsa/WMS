using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Permission : AuditableEntity
    {
        public int Id { get; set; }
        public string PermissionName { get; set; } = string.Empty;

        public List<RolePermission> RolePermissions { get; set; }
    }
}
