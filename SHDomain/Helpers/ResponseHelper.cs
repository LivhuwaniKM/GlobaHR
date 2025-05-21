using SHDomain.Models;

namespace SHDomain.Helpers
{
    public class ResponseHelper : IResponseHelper
    {
        public ApiResponse<T> CreateResponse<T>(bool isSuccess, int statusCode, string message, T? data)
        {
            return new ApiResponse<T>
            {
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        public ApiResponseWithToken<T> CreateResponse<T>(bool isSuccess, int statusCode, string message, T? data, string? token)
        {
            return new ApiResponseWithToken<T>
            {
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Token = token ?? string.Empty
            };
        }
    }
}
