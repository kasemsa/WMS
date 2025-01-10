namespace WarehouseManagementSystem.Models.Responses
{
    public class BaseResponse<T>
    {
        // Default Constructor
        public BaseResponse()
        {
            Success = true;
            StatusCode = 200;
            Message = string.Empty;
        }

        // Constructor for simple success message
        public BaseResponse(string message, bool success = true, int statusCode = 200)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }

        // Constructor for simple success message
        public BaseResponse(T data, string message, bool success = true, int statusCode = 200)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        // Constructor for data with optional pagination and metadata
        public BaseResponse(string message, bool success, int statusCode, T data, Pagination? pagination = null, int totalItem = 0, int? unreadMessages = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Pagination = pagination;
            TotalItem = totalItem;
            CountOfUnReadingMessages = unreadMessages;
        }

        // Properties
        /// <summary>
        /// Indicates the success of the operation.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Message providing additional context about the response.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Total number of items (useful for paginated responses).
        /// </summary>
        public int TotalItem { get; set; } = 0;

        /// <summary>
        /// The response data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Count of unread messages (optional).
        /// </summary>
        public int? CountOfUnReadingMessages { get; set; }

        /// <summary>
        /// List of validation errors (if any).
        /// </summary>
        public List<string>? ValidationErrors { get; set; } = new();

        /// <summary>
        /// Pagination metadata.
        /// </summary>
        public Pagination? Pagination { get; set; }
    }
}
