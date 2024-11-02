using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class UserToken : AuditableEntity
    {
        public int Id {  get; set; }
        public User User { get; set; } = null!;
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public string Token {  get; set; } = string.Empty;

    }
}
