using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class CommissarySeeder
    {
        private readonly WarehouseDbContext _context;

        public CommissarySeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (!_context.Commissaries.Any())
            {
                await _context.Commissaries.AddRangeAsync(
                   
                   new Commissary
                   {
                       // Id = 2,
                       Name = "CommissaryAdmin",
                       PhoneNumber = "1234567890",
                       InvoiceItems = null!,
                       UserId = 2
                   });
                await _context.SaveChangesAsync();
            }
        }
    }
}
