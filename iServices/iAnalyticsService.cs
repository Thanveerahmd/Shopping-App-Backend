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

        Task<ICollection<PageViews>> GetPageViewHistoryOfUser(string UserId);

        Task<ICollection<BuyerSearch>> GetBuyerSearchHistoryOfUser(string UserId);

        Task<List<AdvertScoreDto>> GetAdvertisementToReturn(ICollection<Advertisement> ad,string userId);

    }
}