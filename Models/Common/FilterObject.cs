namespace WarehouseManagementSystem.Models.Common
{
    public class FilterObject
    {
        public List<Filter>? Filters { get; set; }
    }
    public class DateTimeRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Filter
    {
        public string? Key { get; set; }
        public string? Value { get; set; } = null!;
        public DateTimeRange? DateRange { get; set; }
    }
    
}
