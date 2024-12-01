
using Microsoft.VisualBasic.FileIO;

namespace WarehouseManagementSystem.Contract.FileServices
{
    public class FileService : IFileService
    {
        private readonly string _SavePath;

        public FileService(IWebHostEnvironment environment)
        {
            _SavePath = Path.Combine(environment.ContentRootPath + "/wwwroot", "UploadedFiles");

            if (!Directory.Exists(_SavePath))
            {
                Directory.CreateDirectory(_SavePath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var filePath = Path.Combine(_SavePath, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
