using Microsoft.AspNetCore.Http;

namespace SHDomain.Models.Media
{
    public class UserImageUploadDto
    {
        public int EmployeeId { get; set; }
        public required IFormFile Image { get; set; }
    }
}
