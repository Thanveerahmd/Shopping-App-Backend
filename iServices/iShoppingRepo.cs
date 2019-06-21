using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface iShoppingRepo
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetAllProductsOfSeller(string sellerID);
        Task<Product> GetProduct(int id);
        Task<Cart> GetCart(string id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUserAsync(int ProductId);
        void UpdateDeliveryInfo(DeliveryInfo DeliveryInfo);
        Task<IEnumerable<Product>> GetProductsBySearchQuery(string searchQuery,string paramter);
        Task<IEnumerable<DeliveryInfo>> GetDeliveryInfosOfUser(string userId);
        Task<DeliveryInfo> GetDeliveryInfo(int id);
        Task<DeliveryInfo> GetDeliveryInfoOfDefault(string userId);
    }
}