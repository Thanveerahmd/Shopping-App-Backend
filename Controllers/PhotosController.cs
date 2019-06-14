using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Entities;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("photo")]
    public class PhotosController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        private Cloudinary _cloudinary;

        public PhotosController(IMapper mapper, iShoppingRepo repo)
        {
            _repo = repo;
            _mapper = mapper;

            Account acc = new Account(
            Keys.Cloudinary_cloud_name,
            Keys.Cloudinary_api_key,
            Keys.Cloudinary_api_secret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost("addPhoto/{product_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPhotoForProduct(int product_id,
        [FromForm]PhotoUploadDto PhotoUploadDto)
        {
            var product = await _repo.GetProduct(product_id);

            var file = PhotoUploadDto.file;

            var Upload_result = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var UploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                         .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    Upload_result = _cloudinary.Upload(UploadParams);
                }
            }

            PhotoUploadDto.Url = Upload_result.Uri.ToString();
            PhotoUploadDto.PublicID = Upload_result.PublicId;

            var photo = _mapper.Map<Photo>(PhotoUploadDto);

            if (!product.Photos.Any(u => u.isMain))
                photo.isMain = true;

            product.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Coudn't add the Photo");
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }


        [HttpPost("{ProductId}/{id}/setMain")]  // id reffer to Photo id
        [AllowAnonymous]
        public async Task<IActionResult> SetMainPhoto(int ProductId, int id)
        {
            // if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var product = await _repo.GetProduct(ProductId);

            if (!product.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.isMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUserAsync(ProductId);

            if (!(currentMainPhoto == null))
                currentMainPhoto.isMain = false;

            photoFromRepo.isMain = true;

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }


        [HttpDelete("{ProductId}/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeletePhoto(int ProductId, int id)
        {
           
            var product = await _repo.GetProduct(ProductId);

            if (!product.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.isMain)
                return BadRequest("You cannot delete your main photo");

            if (photoFromRepo.PublicID != null)
            {
                var delParams = new DelResParams()
                {
                    PublicIds = new List<string>() {photoFromRepo.PublicID},
                    Invalidate = true
                };
                var delResult = _cloudinary.DeleteResources(delParams);

                if (!delResult.Partial)
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicID == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to delete the photo");
        }


    }
}