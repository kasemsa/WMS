using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase.Seeders
{
    public class UserSeeder
    {
        private readonly WarehouseDbContext _context;

        public UserSeeder(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };
            
            if (!_context.Users.Any())
            {
                await _context.Users.AddRangeAsync(
                   new User
                   {
                       // Id = 1,
                       Name = "Admin",
                       UserName = "Admin",
                       PhoneNumber = "1234567890",
                       Email = "Admin@admin.com",
                       Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: "Admin123",
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8))
                   });
                await _context.SaveChangesAsync();
            }
        }
    }
}
