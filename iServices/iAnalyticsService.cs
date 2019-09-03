using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

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

    }
}