using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class PurchaseInvoice : AuditableEntity
    {
        public int Id { get; set; }

        public ICollection<Product> Products { get; set; }

        public decimal InvoiceTotal { get; set; }
        public Customer Commissary { get; set; }

        [ForeignKey(nameof(Commissary))]
        public int CommissaryId { get; set; }
    }
}
