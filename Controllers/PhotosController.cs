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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("photo")]
    public class PhotosController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        private readonly UserManager<User> _usermanger;
        private Cloudinary _cloudinary;
        private readonly iAdvertisement _adService;

        public PhotosController(IMapper mapper,
         iShoppingRepo repo,
         UserManager<User> usermanger,
         iAdvertisement adService)
        {
            _repo = repo;
            _mapper = mapper;
            _usermanger = usermanger;
            _adService = adService;

            Account acc = new Account(
            Keys.Cloudinary_cloud_name,
            Keys.Cloudinary_api_key,
            Keys.Cloudinary_api_secret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost("addPhoto/{product_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPhotoForProduct(int product_id, [FromForm]PhotoUploadDto PhotoUploadDto)
        {
            var product = await _repo.GetProduct(product_id);

            var file = PhotoUploadDto.file;

            var Upload_result = new ImageUploadResult();

            if (file != null && file.Length > 0)
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
            else
            {
                return BadRequest("your file is corrupted");
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

            var product = await _repo.GetProduct(ProductId);

            if (!product.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.isMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUserAsync(ProductId);

            if (currentMainPhoto != null)
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

            // if (photoFromRepo.isMain)
            //     return BadRequest("You cannot delete your main photo");

            if (photoFromRepo.PublicID != null)
            {
                var delParams = new DelResParams()
                {
                    PublicIds = new List<string>() { photoFromRepo.PublicID },
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

        [HttpPost("addPhotoforuser/{user_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPhotoForUser(string user_id, [FromForm]PhotoUploadDto PhotoUploadDto)
        {
            var user = await _usermanger.FindByIdAsync(user_id);

            var file = PhotoUploadDto.file;

            var Upload_result = new ImageUploadResult();

            if (file != null && file.Length > 0)
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
            else
            {
                return BadRequest(new { message = "Your file is corrupted" });
            }

            PhotoUploadDto.Url = Upload_result.Uri.ToString();

            PhotoUploadDto.PublicID = Upload_result.PublicId;

            var photo = _mapper.Map<PhotoForUser>(PhotoUploadDto);

            photo.UserId = user.Id;

            user.Photo = photo;

            user.imageUrl = photo.Url;

            var result = await _usermanger.UpdateAsync(user);



            if (result.Succeeded)
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return Ok(new { imageUrl = photoToReturn.Url });
            }
            else
            {
                return BadRequest(new { message = "Coudn't add the Photo" });
            }

        }

        [HttpDelete("delete/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUserPhoto(string UserId)
        {

            var user = await _usermanger.FindByIdAsync(UserId);


            var photoFromRepo = await _repo.GetPhotoOfUser(UserId);

            if (photoFromRepo.PublicID != null)
            {
                var delParams = new DelResParams()
                {
                    PublicIds = new List<string>() { photoFromRepo.PublicID },
                    Invalidate = true
                };
                var delResult = _cloudinary.DeleteResources(delParams);

                if (!delResult.Partial)
                {
                    user.imageUrl = null;
                    await _usermanger.UpdateAsync(user);
                    _repo.Delete(photoFromRepo);

                }
            }

            if (photoFromRepo.PublicID == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest(new { message = "Failed to delete photo" });
        }

        [HttpGet("UserImage/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPhoto(string id)
        {
            var photoFromRepo = await _repo.GetPhotoOfUser(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        // CREATE PROCEDURE dbo.DeleteOnSchedule
        //  AS
        // BEGIN
        //  DELETE[dbo].[Table]
        // WHERE[DateTimeToDelete] <= CURRENT_TIMESTAMP;
        //  END
        // automatic delete

        // neee to do testing

        [HttpPost("Advertisement/{AdId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAdvertisementPhoto(int AdId, [FromForm]PhotoUploadDto PhotoUploadDto)
        {

            var file = PhotoUploadDto.file;

            var Upload_result = new ImageUploadResult();

            if (file != null && file.Length > 0)
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
            else
            {
                return BadRequest(new { message = "Your file is corrupted" });
            }

            PhotoUploadDto.Url = Upload_result.Uri.ToString();

            PhotoUploadDto.PublicID = Upload_result.PublicId;

            var photo = _mapper.Map<PhotoForAd>(PhotoUploadDto);

            var ad = await _adService.GetAdvertisement(AdId);

            ad.PublicID = photo.PublicID;
            ad.Url = photo.Url;
            ad.PhotoForAd = photo;

            if (await _adService.UpdateAdvertisement(ad))
            {
                return Ok();
            }
            return BadRequest();
        }
    }

}