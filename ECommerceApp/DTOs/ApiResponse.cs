namespace ECommerceApp.DTOs
{
    // Standardized API response structure.
    //T represents then type of the response data
    public class ApiResponse<T>
    {
        // HTTP status code of the response
        public int StatusCode { get; set; }

        // Indicates whether the request was successful
        public bool Success { get; set; }

        // Response data in case successful
        public T Data { get; set; }

        // List of error messages, if any.
        public List<string> Errors { get; set; }

        // Default constructor
        // Used to create an empty response and fill it later
        public ApiResponse()
        {
            Success = true;
            Errors = new List<string>();
        }

        // Constructor for success response with data
        public ApiResponse(int statusCode, T data)
        {
            StatusCode = statusCode;
            Success = true;
            Data = data;
            Errors = new List<string>();
        }

        // Constructor for error response with multiple errors
        public ApiResponse(int statusCode, List<string> errors)
        {
            StatusCode = statusCode;
            Success = false;
            Errors = errors;
        }

        // Constructor for error response with a single error
        public ApiResponse(int statusCode, string error)
        {
            StatusCode = statusCode;
            Success = false;
            Errors = new List<string>() { error };
        }
    }
}
