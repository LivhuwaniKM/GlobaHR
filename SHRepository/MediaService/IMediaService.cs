using SHDomain.Models;
using SHDomain.Models.Media;

namespace SHServices.MediaService
{
    public interface IMediaService
    {
        Media? GetImageByEmployeeId(int employeeId);
        ApiResponse<bool> UploadImage(UserImageUploadDto imageUploadDto);
        ApiResponse<bool> DeleteImage(int employeeId);
    }
}
