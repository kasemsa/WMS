using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class UserRole : AuditableEntity
    {
        public int Id { get; set; }
        public Role Role { get; set; } = null!;
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public User User { get; set; } = null!;
        [ForeignKey(nameof(User))]
        public int UserId {  get; set; }
    }
}
