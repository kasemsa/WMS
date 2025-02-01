using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class PermissionSeeder
    {
        private readonly WarehouseDbContext _context;
    
        public PermissionSeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.Permissions.Any())
            {
                await _context.Permissions.AddRangeAsync(
                new Permission()
                {
                    //Id = 1,
                    PermissionName = "All"
                },
                //------Commissaries-----
                new Permission()
                {
                    //Id = 2,
                    PermissionName = "GetAllCommissaries"
                },
                new Permission()
                {
                    //Id = 3,
                    PermissionName = "GetCommissaryById"
                },
                new Permission()
                {
                    //Id = 4,
                    PermissionName = "CreateCommissary"
                },
                new Permission()
                {
                    //Id = 5,
                    PermissionName = "UpdateCommissary"
                },
                new Permission()
                {
                    //Id = 6,
                    PermissionName = "DeleteCommissary"
                },
                //------Customer-----
                new Permission()
                {
                    //Id = 7,
                    PermissionName = "CreateCustomer"
                },
                new Permission()
                {
                    //Id = 8,
                    PermissionName = "UpdateCustomer"
                },
                new Permission()
                {
                    //Id = 9,
                    PermissionName = "DeleteCustomer"
                },
                new Permission()
                {
                    //Id = 10,
                    PermissionName = "GetCustomerById"
                },
                new Permission()
                {
                    //Id = 11,
                    PermissionName = "GetAllCustomers"
                },
                //------Product-----
                new Permission()
                {
                    //Id = 12,
                    PermissionName = "CreateProduct"
                },
                new Permission()
                {
                    //Id = 13,
                    PermissionName = "UpdateProduct"
                },
                new Permission()
                {
                    //Id = 14,
                    PermissionName = "DeleteProduct"
                },
                new Permission()
                {
                    //Id = 15,
                    PermissionName = "GetProductById"
                },
                new Permission()
                {
                    //Id = 16,
                    PermissionName = "GetAllProducts"
                },
                //------Users-----
                new Permission()
                {
                    //Id = 17,
                    PermissionName = "CreateUser"
                },
                new Permission()
                {
                    //Id = 18,
                    PermissionName = "UpdateUser"
                },
                new Permission()
                {
                    //Id = 19,
                    PermissionName = "DeleteUser"
                },
                new Permission()
                {
                    //Id = 20,
                    PermissionName = "GetUserById"
                },
                new Permission()
                {
                    //Id = 21,
                    PermissionName = "GetAllUsers"
                },
                //------Invoice-----
                new Permission()
                {
                    //Id = 22,
                    PermissionName = "GetSalesInvoiceById"
                },
                new Permission()
                {
                    //Id = 23,
                    PermissionName = "GetPurchaseInvoiceById"
                },
                new Permission()
                {
                    //Id = 24,
                    PermissionName = "RefundInvoice"
                },
                new Permission()
                {
                    //Id = 25,
                    PermissionName = "RefundPartialInvoice"
                },
                new Permission()
                {
                    //Id = 26,
                    PermissionName = "RefundPurchase"
                },
                new Permission()
                {
                    //Id = 27,
                    PermissionName = "CreateSalesInvoice"
                },
                new Permission()
                {
                    //Id = 28,
                    PermissionName = "CreatePurchaseInvoice"
                },
                new Permission()
                {
                    //Id = 29,
                    PermissionName = "UpdateSalesInvoice"
                },
                new Permission()
                {
                    //Id = 30,
                    PermissionName = "GetAllSalesInvoices"
                },
                new Permission()
                {
                    //Id = 31,
                    PermissionName = "GetAllPurchaseInvoices"
                },
                // ------Print--------
                new Permission()
                {
                    //Id = 30,
                    PermissionName = "PrintSalesInvoiceById"
                },
                new Permission()
                {
                    //Id = 31,
                    PermissionName = "PrintPurchaseInvoiceById"
                },
                // ------Role--------
                new Permission()
                {
                    //Id = 30,
                    PermissionName = "CreateRole"
                },
                new Permission()
                {
                    //Id = 31,
                    PermissionName = "DeleteRole"
                },
                new Permission()
                {
                    //Id = 32,
                    PermissionName = "AddPermissionToRole"
                },
                new Permission()
                {
                    //Id = 33,
                    PermissionName = "AsignPermissionToUser"
                },
                new Permission()
                {
                    //Id = 34,
                    PermissionName = "ApplySeeder"
                });

                await _context.SaveChangesAsync();
            }

        }
    }
}
 