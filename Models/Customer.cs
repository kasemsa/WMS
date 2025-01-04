using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Customer : AuditableEntity
    {
        public Customer()
        {
        }

        public Customer(int id, string name, string city, decimal balance, List<SalesInvoice> salesInvoices)
        {
            Id = id;
            Name = name;
            City = city;
            Balance = balance;
            SalesInvoices = salesInvoices;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public List<SalesInvoice> SalesInvoices { get; set; } = null!;
    }
}
