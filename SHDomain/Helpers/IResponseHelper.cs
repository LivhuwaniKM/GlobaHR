using SHDomain.Models;

namespace SHDomain.Helpers
{
    public interface IResponseHelper
    {
        ApiResponse<T> CreateResponse<T>(bool isSuccess, int statusCode, string message, T? data);
        ApiResponseWithToken<T> CreateResponse<T>(bool isSuccess, int statusCode, string message, T? data, string? token);
    }
}
