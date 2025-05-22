using SHDomain.Models;
using SHDomain.Models.User;

namespace SHServices.UserService
{
    public interface IUserService
    {
        Task<ApiResponseWithToken<UserDto>> LoginUserAsync(Login user);
        Task<ApiResponse<User>> CreateUserAsync(User user);
        Task<ApiResponse<User>> UpdateUserAsync(User user);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<ApiResponse<UserDto>> SearchUserAsync(UserSearchModel searchModel);
        Task<ApiResponse<bool>> DeleteUserAsync(int userId);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto passwModel);
    }
}
