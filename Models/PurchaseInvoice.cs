using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class PurchaseInvoice : AuditableEntity
    {
        public int Id { get; set; }
        public Customer Commissary { get; set; } = null!;
        [ForeignKey(nameof(Commissary))]
        public int CommissaryId { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = null!;
    }
}
