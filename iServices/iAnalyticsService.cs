using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;
using pro.backend.Dtos;

namespace pro.backend.iServices
{
    public interface iAnalytics
    {

        Task<PageViews> GetPageViewRecordIfAvailable(string Sub_Category, string UserId);

        Task<bool> UpdatePageViewRecord(PageViews prevRecord);

        Task<bool> AddPageViewRecord(PageViews prevRecord);

        Task<BuyerSearch> GetBuyerSearchRecordIfAvailable(string Keyword, string UserId);

        Task<bool> UpdateBuyerSearchRecord(BuyerSearch prevRecord);

        Task<bool> AddBuyerSearchRecord(BuyerSearch prevRecord);
        IList<Product> getRecommendation(int currentProductID);

        Task<ICollection<PageViews>> GetPageViewHistoryOfUser(string UserId);

        Task<ICollection<BuyerSearch>> GetBuyerSearchHistoryOfUser(string UserId);

        Task<float> getPriceSuggestions(int Sub_categoryId, string Product_Description, string Product_name);
        Task<List<AdvertScoreDto>> GetAdvertisementToReturn(ICollection<Advertisement> ad, string userId);

        Task<Promo> GetNotificationToReturn(ICollection<Promo> promos, string userId);

        double similarityOfProducts(string ProductName1, string ProductDescription1, string ProductName2, string ProductDescription2);

        IList<Product> GetUserPreference(string UserId);
        Task<bool> UpdateProductViewRecord(ProductView prevRecord);
         Task<ProductView> GetProductViewRecordIfAvailable(int ProductId, string UserId);

    }
}