namespace WarehouseManagementSystem.Models.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<int>? RoleIds { get; set; }
    }
}
