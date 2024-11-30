namespace WarehouseManagementSystem.Infrastructure.ExcelService
{
    public interface IExcelService<T> where T : class
    {
        public byte[] ExportToExcel(List<T> data);
        public byte[] GeneratePrototype();
        public List<T> ImportFromExcel(byte[] fileContent);
    }

}
