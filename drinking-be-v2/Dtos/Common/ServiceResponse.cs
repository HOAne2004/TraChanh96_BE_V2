namespace drinking_be.Dtos.Common
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}