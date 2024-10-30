namespace WarehouseManagementSystem.Models.Common
{
    public class AuditableEntity
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
