﻿using Newtonsoft.Json;

namespace WarehouseManagementSystem.Models.Dtos
{
    public class AsignRoleToUserRequset
    {
        [JsonProperty("UserId")]
        public int UserId { get; set; }

        [JsonProperty("RoleId")]
        public int RoleId { get; set; }
    }
}
