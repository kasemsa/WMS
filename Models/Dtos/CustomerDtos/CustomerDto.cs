namespace WarehouseManagementSystem.Models.Dtos.CustomerDtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<int>? SalesInvoiceIds { get; set; }
    }
}
