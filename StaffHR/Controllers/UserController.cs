using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.User;
using SHServices.UserService;

namespace StaffHR.Controllers
{
    public class UserController(IUserService _userService, IResponseHelper _responseHelper) : BaseController
    {
        [AllowAnonymous]
        [HttpPost("/api/user/login")]
        public async Task<ActionResult> LoginUserAync(Login user)
        {
            var response = await _userService.LoginUserAsync(user);

            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpPost("/api/user/create")]
        public async Task<ActionResult> CreateUserAync(User user)
        {
            var response = await _userService.CreateUserAsync(user);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/user/update/{id}")]
        public async Task<ActionResult> UpdateUserAync(int id, [FromBody] User user)
        {
            if (user == null || user.Id != id)
            {
                var err = _responseHelper.CreateResponse<User>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }
            var response = await _userService.UpdateUserAsync(user);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/user/search")]
        public async Task<ActionResult> SearchUserAsync([FromQuery] UserSearchModel searchModel)
        {
            var response = await _userService.SearchUserAsync(searchModel);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/user/list")]
        public async Task<ActionResult> GetAllUsersAsync()
        {
            var response = await _userService.GetAllUsersAsync();

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost("/api/user/delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUserAsync(int id)
        {
            var response = await _userService.DeleteUserAsync(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/user/reset-password")]
        public async Task<ActionResult> ResetPasswordAsync(ResetPasswordDto passwModel)
        {
            var response = await _userService.ResetPasswordAsync(passwModel);

            return StatusCode(response.StatusCode, response);
        }
    }
}