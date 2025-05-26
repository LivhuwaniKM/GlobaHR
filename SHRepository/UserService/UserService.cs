using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.User;

namespace SHServices.UserService
{
    public class UserService(AppDbContext _db,
        IResponseHelper _responseHelper,
        ILogger<UserService> _logger,
        IMapper _mapper,
        TokenHelper _tokenHelper
        ) : IUserService
    {
        public async Task<ApiResponse<User>> CreateUserAsync(User user)
        {
            if (
                string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Role)
                )
            {
                return _responseHelper.CreateResponse<User>(false, 400, "Invalid request. Null object reference", null);
            }

            try
            {
                var response = await _db.Users.FirstOrDefaultAsync(prop => prop.Email == user.Email.ToLower());

                if (response != null)
                    return _responseHelper.CreateResponse<User>(true, 200, "A matching record has been found.", null);

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                user.Email = user.Email.ToLower();

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 201, "User created successfully.", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<User>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponseWithToken<UserDto>> LoginUserAsync(Login user)
        {
            try
            {
                var validUser = await _db.Users.FirstOrDefaultAsync(c => c.Email.ToLower() == user.Email.ToLower());

                if (validUser != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, validUser.Password);

                    if (!isPasswordValid)
                        return _responseHelper.CreateResponse<UserDto>(false, 404, "Login failed.", null, "");

                    var token = _tokenHelper.GetJwtToken(validUser);

                    var response = _mapper.Map<UserDto>(validUser);

                    return _responseHelper.CreateResponse(true, 200, "Login successful.", response, token);
                }

                return _responseHelper.CreateResponse<UserDto>(false, 404, "Login failed.", null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<UserDto>(false, 500, ex.Message, null, null);
            }
        }

        public async Task<ApiResponse<User>> UpdateUserAsync(User user)
        {
            try
            {
                var validUser = await _db.Users.FirstOrDefaultAsync(prop => prop.Id == user.Id);

                if (validUser != null)
                {
                    validUser.FirstName = user.FirstName;
                    validUser.LastName = user.LastName;
                    validUser.Email = user.Email;
                    validUser.Role = user.Role;
                    validUser.Password = validUser.Password;

                    _db.Entry(validUser).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    return _responseHelper.CreateResponse(true, 200, "User updated successfully.", validUser);
                }

                return _responseHelper.CreateResponse<User>(false, 404, "User not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<User>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var usersList = await _db.Users.Where(u => u.IsDeleted == false).ToListAsync();

                if (usersList == null || usersList.Count == 0)
                    return _responseHelper.CreateResponse<IEnumerable<UserDto>>(false, 404, "Users not found.", null);

                var response = _mapper.Map<List<UserDto>>(usersList);

                return _responseHelper.CreateResponse<IEnumerable<UserDto>>(true, 200, "Users retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<IEnumerable<UserDto>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<UserDto>> SearchUserAsync(UserSearchModel searchModel)
        {
            if (searchModel.UserId <= 0 && string.IsNullOrWhiteSpace(searchModel.Email))
                return _responseHelper.CreateResponse<UserDto>(false, 400, "Invalid request. Null object reference", null);

            try
            {
                var query = _db.Users.AsQueryable().Where(c => (c.Id == searchModel.UserId || c.Email == searchModel.Email) && c.IsDeleted == false);

                var result = await query.FirstOrDefaultAsync();

                if (result != null)
                {
                    var response = _mapper.Map<UserDto>(result);

                    return _responseHelper.CreateResponse(true, 200, "User retrieved successfully.", response);
                }

                return _responseHelper.CreateResponse<UserDto>(false, 404, "User not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<UserDto>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    return _responseHelper.CreateResponse(false, 400, "Invalid input data.", false);

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);

                if (user == null)
                    return _responseHelper.CreateResponse(false, 400, "User not found.", false);

                user.IsDeleted = true;

                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 200, "User deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse(false, 500, ex.Message, false);
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto passwModel)
        {
            if (
                string.IsNullOrWhiteSpace(passwModel.CurrentPassword) ||
                string.IsNullOrWhiteSpace(passwModel.NewPassword) ||
                string.IsNullOrWhiteSpace(passwModel.ConfirmPassword)
                )
            {
                return _responseHelper.CreateResponse<string>(false, 400, "Invalid input data.", null);
            }

            if (passwModel.NewPassword != passwModel.ConfirmPassword)
                return _responseHelper.CreateResponse<string>(false, 400, "Passwords do not match.", null);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == passwModel.Id);

            if (user == null)
                return _responseHelper.CreateResponse<string>(false, 404, "User not found.", null);

            if (!BCrypt.Net.BCrypt.Verify(passwModel.CurrentPassword, user.Password))
                return _responseHelper.CreateResponse<string>(false, 400, "Passwords do not match.", null);

            user.Password = BCrypt.Net.BCrypt.HashPassword(passwModel.CurrentPassword);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return _responseHelper.CreateResponse<string>(true, 200, "Password reset successful.", null);
        }
    }
}
