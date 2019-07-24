using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface iShoppingRepo
    {
        void AddOrder(Order Order);
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void AddAll<T>(ICollection<T> entity) where T : class;
        void DeleteAll<T>(ICollection<T> entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetAllProductsOfSeller(string sellerID);

        Task<IEnumerable<Product>> GetAllUnflaggedProductsOfSeller(string sellerID);

        Task<Product> GetProduct(int id);
        Task<Order> GetOrder(int id);
        Task<Cart> GetCart(string id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUserAsync(int ProductId);
        Task<bool> UpdateDeliveryInfo(DeliveryInfo DeliveryInfo);
        Task<IEnumerable<Product>> GetProductsBySearchQuery(string searchQuery, string paramter);
        Task<ICollection<DeliveryInfo>> GetDeliveryInfosOfUser(string userId);
        Task<DeliveryInfo> GetDeliveryInfo(int id);
        Task<DeliveryInfo> GetDeliveryInfoOfDefault(string userId);
        Task<DeliveryInfo> SetAlternateDefault(string userId);
        Task<CartProduct> GetCartProduct(int CartproductId);
        Task<ICollection<CartProduct>> GetAllCartProduct(int CartId);
        Task UpdateCartDetails(CartProduct CartProduct);
        Task<CartProduct> FindProductMatchInCart(int productId, int cartId);
        Task<bool> UpdateRating(Rating rating);
        Task<Rating> GetRatingById(Rating rating);
        Task<PhotoForUser> GetPhotoOfUser(string UserId);
        Task<ICollection<BillingInfo>> GetBillingInfosOfUser(string userId);
        Task<BillingInfo> GetBillingInfo(int id);
        Task<BillingInfo> GetBillingInfobyOtp(string userId, string otp);
        Task<BillingInfo> GetBillingInfoOfDefault(string userId);
        Task<BillingInfo> AlternateDefault(string userId);
        Task<bool> UpdateBillingInfo(BillingInfo BillingInfo);
        Task<bool> UpdateOrder(Order Order);
        Task<bool> UpdateSellerInfo(SellerPaymentInfo SellerPaymentInfo);
        Task<bool> UpdateBuyerInfo(BuyerPaymentInfo BuyerPaymentInfo);
    }
}