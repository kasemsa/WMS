using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Infrastructure.JwtService
{
    public interface IJwtProvider
    {
        public string Generate(User user);
        public int GetUserIdFromToken(string token);
    }
}
