namespace WarehouseManagementSystem.Contract.FileService
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
