namespace WarehouseManagementSystem.Models.Responses
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            success = true;
        }
        public BaseResponse(string message)
        {
            success = true;
            statusCode = 200;
            this.message = message;
        }

        public BaseResponse(string message, bool success, int statusCode)
        {
            this.success = success;
            this.statusCode = statusCode;
            this.message = message;
        }
        public BaseResponse(string message, bool success, int statusCode, T data)
        {
            this.statusCode = statusCode;
            this.success = success;
            this.message = message;
            this.data = data;
        }
        public BaseResponse(string message, bool success, int statusCode, T data, int count)
        {
            this.statusCode = statusCode;
            this.success = success;
            this.message = message;
            this.data = data;
            this.totalItem = count;
        }
        public BaseResponse(string message, bool success, int statusCode, T data, Pagination? pagination)
        {
            this.statusCode = statusCode;
            this.success = success;
            this.message = message;
            this.data = data;
            this.pagination = pagination;
        }
        public BaseResponse(string message, bool success, int statusCode, T data, Pagination? pagination, int CountOfUnReadingMessages)
        {
            this.statusCode = statusCode;
            this.success = success;
            this.message = message;
            this.data = data;
            this.pagination = pagination;
            this.CountOfUnReadingMessages = CountOfUnReadingMessages;
        }
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; } = string.Empty;
        public int totalItem { get; set; }
        public T? data { get; set; }
        public int? CountOfUnReadingMessages { get; set; }
        public List<string>? validationErrors { get; set; }
        public Pagination? pagination { get; set; }
    }
}
