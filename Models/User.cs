using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class User : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public List<UserRole> UserRoles { get; set; } = null!;
        public List<UserPermission>? Permissions { get; set; }

        public Commissary Commissary { get; set; } = null!;
    }
}
