using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class UserPermission : AuditableEntity
    {
        public int Id { get; set; }
        public User User { get; set; } = null!;
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public Permission Permission { get; set; } = null!;
        [ForeignKey(nameof(Permission))]
        public int PermissionId { get; set; }
    }
}
