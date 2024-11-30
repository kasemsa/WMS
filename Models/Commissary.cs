using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Commissary : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public required List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    }
}
