namespace WarehouseManagementSystem.Models.Responses
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = null!;
        public List<Permission> Permissions { get; set; } = null!;
        public string? Token { get; set; } = string.Empty;
    }
}
