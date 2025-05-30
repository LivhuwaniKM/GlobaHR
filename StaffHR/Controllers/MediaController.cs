﻿using Microsoft.AspNetCore.Mvc;
using SHDomain.Models.Media;
using SHServices.MediaService;

namespace StaffHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController(IMediaService _mediaService) : ControllerBase
    {
        [HttpGet("image/{employeeId}")]
        public ActionResult GetImageByEmployeeId(int employeeId)
        {
            var image = _mediaService.GetImageByEmployeeId(employeeId);

            if (image == null || image.FileData == null)
                return NotFound("Image not found");

            return File(image.FileData, image.ContentType, image.FileName);
        }

        [HttpPost("upload")]
        public ActionResult UploadImage([FromForm] UserImageUploadDto dto)
        {
            var response = _mediaService.UploadImage(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete/{employeeId}")]
        public ActionResult DeleteImage(int employeeId)
        {
            var response = _mediaService.DeleteImage(employeeId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("list")]
        public ActionResult<List<ImageFileDto>> GetAllImages()
        {
            var images = _mediaService.GetAllImages();

            if (images == null || images.Count == 0)
                return NotFound("No images found.");

            var imageDtos = images.Select(img => new ImageFileDto
            {
                FileName = img.FileName,
                ContentType = img.ContentType,
                Base64Data = Convert.ToBase64String(img.FileData),
            }).ToList();

            return Ok(imageDtos);
        }
    }
}
