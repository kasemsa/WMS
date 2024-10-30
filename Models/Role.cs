using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Role : AuditableEntity
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public List<UserRole> UserRoles { get; set; } = null!;
        public List<RolePermission> RolePermissions { get; set; } = null!;
    }
}
