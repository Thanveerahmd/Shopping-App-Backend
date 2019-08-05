using pro.backend.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pro.backend.iServices
{
    public interface iCategoryService
    {

        Task<ICollection<Category>> GetCategory();
        Task<bool> IsProductAvailable(int CategoryId);
        Task<ICollection<SubCategory>> GetSubCategoryswithPhoto(int CategoryId);
        Task<SubCategory> GetSubCategory(int SubCategoryId);
        Task<bool> UpdateCategory(Category category);
        Task<bool> IsProductAvailableForSubCategory(int CategoryId);
        Task<bool> UpdateSubCategory(SubCategory category);
        Task<ICollection<SubCategory>> GetSubCategoryWithProduct(int CategoryId);
        Task<PhotoForCategory> GetPhotoOfCategory(int id);
        Task<bool> IsSubCategoryAvailable(string SubCategoryName);
        Task<SubCategory> GetSubCategorywithPhoto(int SubCategoryId);
        Task<bool> IsCategoryAvailable(string CategoryName);
        Task<Category> GettheCategory(int CategoryId);
    }
}