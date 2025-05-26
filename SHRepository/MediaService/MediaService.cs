using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.Media;

namespace SHServices.MediaService
{
    public class MediaService(AppDbContext _db, IResponseHelper _response) : IMediaService
    {
        public Media? GetImageByEmployeeId(int employeeId)
        {
            return _db.Media.FirstOrDefault(c => c.EmployeeId == employeeId && c.IsDeleted == false) ?? null;
        }

        public ApiResponse<bool> UploadImage(UserImageUploadDto imageUploadDto)
        {
            var ms = new MemoryStream();
            imageUploadDto.Image.CopyTo(ms);

            Media image = new()
            {
                EmployeeId = imageUploadDto.EmployeeId,
                FileData = ms.ToArray(),
                ContentType = imageUploadDto.Image.ContentType,
                FileExtension = Path.GetExtension(imageUploadDto.Image.FileName),
                FileName = Path.GetFileName(imageUploadDto.Image.FileName),
                IsDeleted = false
            };

            var response = _db.Media.Add(image);
            _db.SaveChanges();

            return response != null
                ? _response.CreateResponse(true, 200, "Image uploaded.", true)
                : _response.CreateResponse(false, 404, "Image not uploaded.", false);
        }

        public ApiResponse<bool> DeleteImage(int employeeId)
        {
            var image = GetImageByEmployeeId(employeeId);
            if (image == null)
                return _response.CreateResponse(false, 404, "Image not found.", false);

            image.IsDeleted = true;
            _db.SaveChanges();

            return _response.CreateResponse(true, 200, "Image deleted successfully.", true);
        }
    }
}
