using pro.backend.Entities;
using Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.iServices;
using System;

namespace pro.backend.Services
{
    public class CategoryService : iCategoryService
    {
        private DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetCategory()
        {
            var categorys = await _context.Category.Include(q => q.SubCategorys).ToListAsync();

            return categorys;
        }
        // get category with subcategory
        public async Task<Category> GettheCategory(int CategoryId)
        {
            var category = await _context.Category.Include(p => p.SubCategorys).Include(p => p.SubCategorys).FirstOrDefaultAsync(p => p.Id == CategoryId);

            return category;
        }
        public async Task<SubCategory> GetSubCategory(int SubCategoryId)
        {
            var Subcategory = await _context.SubCategory.FindAsync(SubCategoryId);

            return Subcategory;
        }
        public async Task<bool> IsSubCategoryAvailable(string SubCategoryName)
        {
            var Subcategory = await _context.SubCategory.FirstOrDefaultAsync(p => p.SubCategoryName == SubCategoryName);

            if (Subcategory != null)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> IsCategoryAvailable(string CategoryName)
        {
            var category = await _context.Category.FirstOrDefaultAsync(p => p.CategoryName == CategoryName);

            if (category != null)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> IsProductAvailable(int CategoryId)
        {
            var category =await  GettheCategory(CategoryId);
            
            if (category.SubCategorys.Any(p => p.Products.Count != 0))
            {
                return true;
            }
            return false;
        }
        public async Task<bool> IsProductAvailableForSubCategory(int CategoryId)
        {
            var category = await _context.SubCategory.FindAsync(CategoryId);

            if (category.Products.Count!=0)
            {
                return true;
            }
            return false;
        }
         public async Task<SubCategory> GetSubCategorywithPhoto(int SubCategoryId)
        {
            var Subcategory = await _context.SubCategory.Include(p => p.PhotoForCategory).Include(p => p.Products).FirstOrDefaultAsync(p => p.Id == SubCategoryId);

            return Subcategory;
        }
        public async Task<ICollection<SubCategory>> GetSubCategoryswithPhoto(int CategoryId)
        {
            var Subcategorys = await _context.SubCategory.Where(p => p.CategoryId == CategoryId).Include(p => p.PhotoForCategory).Include(p => p.Products).ToListAsync();

            return Subcategorys;
        }
        public async Task<ICollection<SubCategory>> GetSubCategoryWithProduct(int CategoryId)
        {
            var Subcategorys = await _context.SubCategory.Where(p => p.CategoryId == CategoryId).Include(p => p.Products).ToListAsync();

            return Subcategorys;
        }
        public async Task<bool> UpdateCategory(Category category)
        {
            var oldcategory = await _context.Category.FindAsync(category.Id);

            if (oldcategory == null)
                throw new AppException("Category not found");

            oldcategory.CategoryName = category.CategoryName;

            _context.Category.Update(oldcategory);

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateSubCategory(SubCategory category)
        {
            var oldcategory = await _context.SubCategory.FindAsync(category.Id);

            if (oldcategory == null)
                throw new AppException("Category not found");

            oldcategory.url = category.url;
            oldcategory.SubCategoryName = category.SubCategoryName;
            oldcategory.PhotoForCategory = category.PhotoForCategory;
            //  oldcategory.isProductAvilable = category.isProductAvilable;
            _context.SubCategory.Update(oldcategory);

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<PhotoForCategory> GetPhotoOfCategory(int id)
        {
            var PhotoForAd = await _context.photoForCategories.FirstOrDefaultAsync(i => i.Id == id);
            return PhotoForAd;
        }
          public async Task<ICollection<Product>> GetProductInAccordingToSales(int SubCategoryId)
        {
            var listOfProducts = await _context.Products.Where(p => p.Sub_categoryId == SubCategoryId).Where(P=> P.NumberOfSales >0 ).OrderByDescending(P=> P.NumberOfSales).ToListAsync();
         
            return listOfProducts;
        }
    }

}