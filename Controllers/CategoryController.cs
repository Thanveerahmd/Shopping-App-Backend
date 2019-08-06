using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;
using Project.Entities;
using Microsoft.AspNetCore.Identity;
using Project.Helpers;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("category")]
    public class CategoryController : ControllerBase
    {
        private readonly iProductService _productService;
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        private readonly iCategoryService _categoryService;
        private readonly UserManager<User> _usermanger;
        private readonly iAdvertisement _adService;
        private readonly PhotosController _photosController;

        public CategoryController(iProductService productService,
        IMapper mapper, iShoppingRepo repo, iCategoryService categoryService,
        iAdvertisement adService, UserManager<User> usermanger, PhotosController photosController)
        {
            _mapper = mapper;
            _repo = repo;
            _categoryService = categoryService;
            _productService = productService;
            _adService = adService;
            _usermanger = usermanger;
            _photosController = photosController;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            if (await _categoryService.IsCategoryAvailable(category.CategoryName))
            {
                return BadRequest(new { message = "There is a Category with same name" });
            }
            else
            {
                _repo.Add(category);
            }

            if (await _repo.SaveAll())
            {
                return Ok(category.Id);
            }
            return BadRequest();
        }

        [HttpPost("addsubcategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddSubCategory(SubCategoryDto categoryDto, int categoryId)
        {
            var category = await _categoryService.GettheCategory(categoryId);

            var subCategory = _mapper.Map<SubCategory>(categoryDto);

            subCategory.CategoryId = categoryId;

            if (category == null)
            {
                return BadRequest(new { message = "There is No Category" });
            }

            if (await _categoryService.IsSubCategoryAvailable(subCategory.SubCategoryName))
            {
                return BadRequest(new { message = "There is a Category with same name" });
            }

            _repo.Add(subCategory);
            category.SubCategorys.Add(subCategory);

            if (await _repo.SaveAll())
            {
                return Ok(subCategory.Id);
            }
            return BadRequest();


        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var Category = await _categoryService.GetCategory();
            var category = _mapper.Map<ICollection<CategoryDto>>(Category);
            return Ok(category);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategorywithSubCategories()
        {
            var Category = await _categoryService.GetCategory();
            var category = _mapper.Map<ICollection<CategoryReturnDto>>(Category);
            return Ok(category);
        }

        [HttpGet("getSubCategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubCategory(int categoryId)
        {
            var Category = await _categoryService.GetSubCategory(categoryId);
            var Subcategory = _mapper.Map<ICollection<SubCategoryDto>>(Category);
            return Ok(Subcategory);
        }

        [HttpDelete("{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _categoryService.GettheCategory(categoryId);

            if (category == null)
            {
                return BadRequest(new { Message = "No Such Category" });
            }

            // if (await _categoryService.IsProductAvailable(categoryId))
            // {
            //     return Unauthorized();
            // }

            var Subcategorys = await _categoryService.GetSubCategoryswithPhoto(categoryId);

            foreach (var item in Subcategorys)
            {
                var subCategory = await _categoryService.GetSubCategorywithPhoto(item.Id);
                
                if (await _categoryService.IsProductAvailableForSubCategory(item.Id))
                return Unauthorized();

            }


            foreach (var item in Subcategorys)
            {
                var subCategory = await _categoryService.GetSubCategorywithPhoto(item.Id);
                
                
                var photo = subCategory.PhotoForCategory;
                if(photo!=null)
                await _photosController.DeleteCategoryPhoto(photo.Id);
            }

            _repo.DeleteAll(Subcategorys);
            _repo.Delete(category);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();
        }

        [HttpDelete("subcategory/{categoryId}/{SubcategoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteSubCategory(int categoryId, int SubcategoryId)
        {

            var category = await _categoryService.GettheCategory(categoryId);

            if (!category.SubCategorys.Any(p => p.Id == SubcategoryId))
                return Unauthorized();

            var Subcategory = await _categoryService.GetSubCategorywithPhoto(SubcategoryId);

            if (await _categoryService.IsProductAvailableForSubCategory(SubcategoryId))
                return Unauthorized();

            var photo = Subcategory.PhotoForCategory;

            if(photo!=null)
            await _photosController.DeleteCategoryPhoto(photo.Id);
            _repo.Delete(Subcategory);

            if (await _repo.SaveAll())
                return Ok(photo.Id);
            return BadRequest();
        }

        [HttpPut("updateCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCategory([FromBody]CategoryDto categoryDto)
        {
            // map dto to entity and set id
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = categoryDto.Id;

            // if (await _categoryService.IsProductAvailable(category.Id))
            // {
            //     return Unauthorized();
            // }
            var Subcategorys = await _categoryService.GetSubCategoryswithPhoto(categoryDto.Id);


            foreach (var item in Subcategorys)
            {
                var subCategory = await _categoryService.GetSubCategorywithPhoto(item.Id);
                
                if (await _categoryService.IsProductAvailableForSubCategory(item.Id))
                return Unauthorized();

            }

            await _categoryService.UpdateCategory(category);


            if (await _repo.SaveAll())
            {
                return Ok(category.Id);
            }
            return BadRequest();

        }

        [HttpPut("updateSubCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateSubCategory([FromBody]SubCategoryDto subCategoryDto)
        {
            // map dto to entity and set id
            var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
            subCategory.Id = subCategoryDto.Id;
            
            var subCategoryCheck = await _categoryService.GetSubCategorywithPhoto(subCategoryDto.Id);

            if (await _categoryService.IsProductAvailableForSubCategory(subCategory.Id))
                return Unauthorized();


            await _categoryService.UpdateSubCategory(subCategory);

            if (await _repo.SaveAll())
            {
                return Ok(subCategory.Id);
            }
            return BadRequest();


        }
    }


}

