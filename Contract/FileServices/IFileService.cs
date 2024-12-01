namespace WarehouseManagementSystem.Contract.FileServices
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
