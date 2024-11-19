using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Models.Constants
{
    public class IndexQuery
    {
        public List<Filter>? filters { get; set; }
        public int page {  get; set; } = 1;
        public int perPage { get; set; } = 10;
    }
}
