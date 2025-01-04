namespace WarehouseManagementSystem.Models
{
    public interface IInvoice
    {
        public int Id { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
