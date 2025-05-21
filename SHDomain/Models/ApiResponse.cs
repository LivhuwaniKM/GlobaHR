namespace SHDomain.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; } = default;
    }

    public class ApiResponseWithToken<T> : ApiResponse<T>
    {
        public string Token { get; set; } = string.Empty;
    }
}
