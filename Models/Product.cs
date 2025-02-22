﻿using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models
{
    public class Product : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public string? Image { get; set; } 

    }
}
