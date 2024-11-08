using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class RolePermission : AuditableEntity
    {
        public int Id { get; set; }

        public Role Role { get; set; } = null!;
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        public Permission Permission { get; set; } = null!;
        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; set; }
    }
}
