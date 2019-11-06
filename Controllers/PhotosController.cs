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
using System.Net.Http;
using pro.backend.Services;
using Microsoft.AspNetCore.Identity;
using Google.Cloud.Vision.V1;
using Project.Helpers;

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
        private readonly iCategoryService _categoryService;
        private readonly iProductService _productService;

        private static readonly HttpClient Client = new HttpClient();

        public PhotosController(IMapper mapper,
        iProductService productService,
         iShoppingRepo repo,
         UserManager<User> usermanger,
         iAdvertisement adService,
         iCategoryService categoryService)
        {
            _repo = repo;
            _mapper = mapper;
            _usermanger = usermanger;
            _adService = adService;
            _categoryService = categoryService;
            _productService = productService;

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

            var value = new
            {
                DataRepresentation = "URL",
                Value = Upload_result.Uri.ToString()
            };

            if (product.visibility)
            {
                Image image3 = Image.FromUri(Upload_result.Uri.ToString());

                ImageAnnotatorClient client = await ImageAnnotatorClient.CreateAsync();

                SafeSearchAnnotation annotation = await client.DetectSafeSearchAsync(image3);

                var adult_content = annotation.Adult;
                var Spoof = annotation.Spoof;
                var Medical = annotation.Medical;
                var Violence = annotation.Violence;
                var Racy = annotation.Racy;

                bool Adult_flag = (adult_content == Likelihood.Unlikely || adult_content == Likelihood.VeryUnlikely);
                bool Spoof_flag = (Spoof == Likelihood.Unlikely || Spoof == Likelihood.VeryUnlikely);
                bool Medical_flag = (Medical == Likelihood.Unlikely || Medical == Likelihood.VeryUnlikely);
                bool Violence_flag = (Violence == Likelihood.Unlikely || Violence == Likelihood.VeryUnlikely);
                bool Racy_flag = (Racy == Likelihood.Unlikely || Racy == Likelihood.VeryUnlikely);

                product.visibility = product.visibility && false;

                if (Adult_flag && Spoof_flag && Medical_flag && Violence_flag && Racy_flag)
                {
                    product.visibility = product.visibility && true;
                }
            }

            // product.visibility = true;
            try
            {
                await _productService.UpdateProduct(product);
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
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

                return Ok(new { visibility = product.visibility });
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

            Image image3 = Image.FromUri(Upload_result.Uri.ToString());

            ImageAnnotatorClient client = await ImageAnnotatorClient.CreateAsync();

            SafeSearchAnnotation annotation = await client.DetectSafeSearchAsync(image3);

            var adult_content = annotation.Adult;
            var Spoof = annotation.Spoof;
            var Medical = annotation.Medical;
            var Violence = annotation.Violence;
            var Racy = annotation.Racy;

            bool Adult_flag = (adult_content == Likelihood.Unlikely || adult_content == Likelihood.VeryUnlikely);
            bool Spoof_flag = (Spoof == Likelihood.Unlikely || Spoof == Likelihood.VeryUnlikely);
            bool Medical_flag = (Medical == Likelihood.Unlikely || Medical == Likelihood.VeryUnlikely);
            bool Violence_flag = (Violence == Likelihood.Unlikely || Violence == Likelihood.VeryUnlikely);
            bool Racy_flag = (Racy == Likelihood.Unlikely || Racy == Likelihood.VeryUnlikely);

            bool safe = false;

            if (Adult_flag && Spoof_flag && Medical_flag && Violence_flag && Racy_flag)
            {
                safe = true;
            }

            if (safe == false)
            {

                var delParams = new DelResParams()
                {
                    PublicIds = new List<string>() { Upload_result.PublicId },
                    Invalidate = true
                };
                var delResult = _cloudinary.DeleteResources(delParams);

                return BadRequest(new { message = "Profile Picture found to be offensive" });
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
                return StatusCode(401, new { message = "Coudn't add the Photo" });
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

        [HttpPost("advertisement/{AdId}")]
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
                        File = new FileDescription(file.Name, stream)
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

            photo.UserId = ad.UserId;
            ad.PublicID = photo.PublicID;
            ad.Url = photo.Url;
            ad.PhotoForAd = photo;

            if (await _adService.UpdateAdvertisement(ad))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("deleteAd/{sellerId}/{AdId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAdPhoto(string sellerId, int AdId)
        {

            var Ad = await _adService.GetAdvertisement(AdId);

            var photoFromRepo = await _adService.GetPhotoOfad(AdId);

            if (photoFromRepo.UserId != sellerId)
            {
                return BadRequest(new { message = "Not Authorized to delete" });
            }


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
                    Ad.Url = null;
                    await _adService.UpdateAdvertisement(Ad);
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

        [HttpPost("subCategory/{SubCategoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddSubCategoryPhoto(int SubCategoryId, [FromForm]PhotoUploadDto PhotoUploadDto)
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

            var photo = _mapper.Map<PhotoForCategory>(PhotoUploadDto);

            var SubCategory = await _categoryService.GetSubCategory(SubCategoryId);

            photo.SubCategoryId = SubCategoryId;

            SubCategory.url = photo.Url;
            SubCategory.PhotoForCategory = photo;
            if (await _categoryService.UpdateSubCategory(SubCategory))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("deleteSubCategory/{imageId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCategoryPhoto(int imageId)
        {

            var photoFromRepo = await _categoryService.GetPhotoOfCategory(imageId);

            var SubCategory = await _categoryService.GetSubCategory(photoFromRepo.SubCategoryId);


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
                    if (SubCategory != null)
                        SubCategory.url = null;

                    await _categoryService.UpdateSubCategory(SubCategory);
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
    }

}