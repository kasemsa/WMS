using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class PurchaseInvoice : AuditableEntity, IInvoice
    {
        public int Id { get; set; }
        public Commissary Commissary { get; set; } = null!;
        [ForeignKey(nameof(Commissary))]
        public int CommissaryId { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = null!;
    }
}
