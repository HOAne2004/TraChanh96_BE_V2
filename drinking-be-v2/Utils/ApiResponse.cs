using System;

namespace drinking_be.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Success = true;
            Message = null;
            Data = default;
            Timestamp = DateTime.UtcNow;
        }

        public ApiResponse(T? data, string? message = null, bool success = true)
        {
            Data = data;
            Message = message;
            Success = success;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> Ok(T? data, string? message = null) =>
            new ApiResponse<T>(data, message, true);

        public static ApiResponse<T> Fail(string? message = null) =>
            new ApiResponse<T>(default, message, false);
    }
}