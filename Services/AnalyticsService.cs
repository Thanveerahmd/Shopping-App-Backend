using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;

using System.Data;
using AutoMapper;
using NReco.CF.Taste.Impl.Similarity;
using NReco.CF.Taste.Impl.Recommender;
using System.Text;
using pro.backend.Dtos;

namespace pro.backend.Services
{
    public class AnalyticsService : iAnalytics
    {
        private readonly DataContext _context;
        private iOrderService _order;
        public readonly iShoppingRepo _repo;
        private readonly IMapper _mapper;


        public AnalyticsService(
            DataContext context,
            iShoppingRepo repo,
            IMapper mapper,
            iOrderService orderService)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
            _order = orderService;
        }

        public async Task<bool> AddBuyerSearchRecord(BuyerSearch prevRecord)
        {
            _context.Add(prevRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddPageViewRecord(PageViews prevRecord)
        {
            _context.Add(prevRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<BuyerSearch>> GetBuyerSearchHistoryOfUser(string UserId)
        {
            var data = await _context.BuyerSearch.Where(i => i.UserId == UserId).ToListAsync();
            return data;
        }

        public async Task<BuyerSearch> GetBuyerSearchRecordIfAvailable(string Keyword, string UserId)
        {
            var data = await _context.BuyerSearch.FirstOrDefaultAsync(i => i.Keyword == Keyword && i.UserId == UserId);
            return data;
        }

        public async Task<ICollection<PageViews>> GetPageViewHistoryOfUser(string UserId)
        {
            var data = await _context.PageViews.Where(i => i.UserId == UserId).ToListAsync();
            return data;
        }

        public async Task<PageViews> GetPageViewRecordIfAvailable(string Sub_category, string UserId)
        {
            var data = await _context.PageViews.FirstOrDefaultAsync(i => i.Sub_category == Sub_category && i.UserId == UserId);
            return data;
        }

        public async Task<bool> UpdateBuyerSearchRecord(BuyerSearch prevRecord)
        {
            prevRecord.LatestVisit = DateTime.Now;
            prevRecord.NoOfSearch++;
            _context.Update(prevRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePageViewRecord(PageViews prevRecord)
        {
            prevRecord.LatestVisit = DateTime.Now;
            prevRecord.NoOfVisits++;
            _context.Update(prevRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public IDataModel Load(string PrefValFld = null)
        {
            var hasPrefVal = !String.IsNullOrEmpty(PrefValFld);

            FastByIDMap<IList<IPreference>> data = new FastByIDMap<IList<IPreference>>();

            var orders = _order.GetAllOrderDetails();

            foreach (var item in orders)
            {

                byte[] bytes = Encoding.ASCII.GetBytes(item.BuyerId);
                long userID = BitConverter.ToInt64(bytes, 0);
                long itemID = item.ProductId; // encoding

                /*
                    Decoding
                    int i = result;
                    byte[] bytes2 = BitConverter.GetBytes(i);
                    string s2 = Encoding.ASCII.GetString(bytes);
                 */

                var userPrefs = data.Get(userID);
                if (userPrefs == null)
                {
                    userPrefs = new List<IPreference>(6);
                    data.Put(userID, userPrefs);
                }

                if (hasPrefVal)
                {
                    var prefVal = Convert.ToSingle(PrefValFld);
                    userPrefs.Add(new GenericPreference(userID, itemID, prefVal));
                }
                else
                {
                    userPrefs.Add(new BooleanPreference(userID, itemID));
                }
            }

            var newData = new FastByIDMap<IPreferenceArray>(data.Count());
            foreach (var entry in data.EntrySet())
            {
                var prefList = (List<IPreference>)entry.Value;
                newData.Put(entry.Key, hasPrefVal ?
                    (IPreferenceArray)new GenericUserPreferenceArray(prefList) :
                    (IPreferenceArray)new BooleanUserPreferenceArray(prefList));
            }
            return new GenericDataModel(newData);
        }

        public IDataModel LoadOrdersDataModel()
        {

            IDataModel dataModel = null;
            dataModel = Load();
            return dataModel;
        }


        public IDataModel GetDataModelForNewUser(IDataModel baseModel, params long[] preferredItems)
        {
            var plusAnonymModel = new PlusAnonymousUserDataModel(baseModel);
            var prefArr = new BooleanUserPreferenceArray(preferredItems.Length);
            prefArr.SetUserID(0, PlusAnonymousUserDataModel.TEMP_USER_ID);
            for (int i = 0; i < preferredItems.Length; i++)
            {
                prefArr.SetItemID(i, preferredItems[i]);
            }
            plusAnonymModel.SetTempPrefs(prefArr);
            return plusAnonymModel;
        }



        public IList<Product> getRecommendation(int currentProductID)
        {

            var ordersDataModel = LoadOrdersDataModel();

            var modelWithCurrentUser = GetDataModelForNewUser(ordersDataModel, currentProductID);

            var similarity = new LogLikelihoodSimilarity(modelWithCurrentUser);

            // in this example, we have no preference values (scores)
            // to get correct results 'BooleanfPref' recommenders should be used

            var recommender = new GenericBooleanPrefItemBasedRecommender(modelWithCurrentUser, similarity);

            var recommendedItems = recommender.Recommend(PlusAnonymousUserDataModel.TEMP_USER_ID, 6, null);
            //IList<long> list = new List<long>();
            IList<Product> list = new List<Product>();
            foreach (var ri in recommendedItems)
            {
                list.Add(_repo.GetProduct((int)ri.GetItemID()).Result);
            }
            return list;
        }
        public async Task<List<AdvertScoreDto>> GetAdvertisementToReturn(ICollection<Advertisement> ad, string userId)
        {
            int startIndex = 0;

            if (userId != "")
            {
                if (ad.Count > 50)
                {
                    int randomGeneratorLimit = ad.Count / 50;
                    Random random = new Random();

                    startIndex = random.Next(randomGeneratorLimit) * 50;
                }

                IList<AdvertScoreDto> filteredAdverts = new List<AdvertScoreDto>();
                var searchQuery = await GetBuyerSearchHistoryOfUser(userId);
                var pageViews = await GetPageViewHistoryOfUser(userId);

                for (int i = startIndex; i < ad.Count && i < startIndex + 50; i++)
                {
                    var item = ad.ElementAt(i);
                    var filter = _mapper.Map<AdvertScoreDto>(item);
                    if (item != null)
                    {
                        var product = await _repo.GetProduct(item.ProductId);
                        if (product != null)
                        {
                            filter.ProductName = product.Product_name;
                            filter.ProductDescription = product.Product_Discription;
                            filter.ProductPrice = product.Price;
                            filter.ProductSubCategory = product.Sub_category;
                            filter.Score = 0;
                        }

                        int searchQueryScore = 0;
                        int pageViewScore = 0;
                        foreach (var value in searchQuery)
                        {
                            if (filter.ProductName != null && filter.ProductDescription != null)
                            {
                                TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                                var time = Convert.ToInt32(timeDiff.TotalDays);
                                if (filter.ProductName.Contains(value.Keyword))
                                {
                                    searchQueryScore += ((2 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                                }

                                if (filter.ProductDescription.Contains(value.Keyword))
                                {
                                    searchQueryScore += ((1 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                                }
                            }


                        }

                        foreach (var value in pageViews)
                        {
                            if (filter.ProductName != null && filter.ProductDescription != null)
                            {
                                TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                                var time = Convert.ToInt32(timeDiff.TotalDays);
                                if (filter.ProductSubCategory.Equals(value.Sub_category))
                                {
                                    pageViewScore += ((3 * value.NoOfVisits) + (time < 7 ? 5 : 0));
                                    break;
                                }
                            }

                        }
                        filter.Score = searchQueryScore + pageViewScore;
                        if (item.Url != null && item.ProductId != 0)
                            filteredAdverts.Add(filter);
                    }

                }



                // foreach (var record in filteredAdverts)
                // {
                //     int searchQueryScore = 0;
                //     int pageViewScore = 0;
                //     foreach (var value in searchQuery)
                //     {
                //         if (record.ProductName != null && record.ProductDescription != null)
                //         {
                //             TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                //             var time = Convert.ToInt32(timeDiff.TotalDays);
                //             if (record.ProductName.Contains(value.Keyword))
                //             {
                //                 searchQueryScore += ((2 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                //             }

                //             if (record.ProductDescription.Contains(value.Keyword))
                //             {
                //                 searchQueryScore += ((1 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                //             }
                //         }


                //     }

                //     foreach (var value in pageViews)
                //     {
                //         if (record.ProductName != null && record.ProductDescription != null)
                //         {
                //             TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                //             var time = Convert.ToInt32(timeDiff.TotalDays);
                //             if (record.ProductSubCategory.Equals(value.Sub_category))
                //             {
                //                 pageViewScore += ((3 * value.NoOfVisits) + (time < 7 ? 5 : 0));
                //             }
                //         }

                //     }
                //     record.Score = searchQueryScore + pageViewScore;
                // }

                var finalScoredAdverts = filteredAdverts.OrderByDescending(p => p.Score);
                var dataToSend = finalScoredAdverts.Take(6);
                var list = dataToSend.ToList();

                return list;

            }
            else
            {
                IList<AdvertScoreDto> adsToView = new List<AdvertScoreDto>();

                for (int i = startIndex; i < 5; i++)
                {
                    var item = ad.ElementAt(i);
                    var adtoReturn = _mapper.Map<AdvertScoreDto>(item);
                    if (item != null)
                    {
                        var product = await _repo.GetProduct(item.ProductId);
                        if (product != null)
                        {
                            adtoReturn.ProductName = product.Product_name;
                            adtoReturn.ProductPrice = product.Price;
                            adtoReturn.ProductDescription = product.Sub_category;
                        }
                    }

                    adsToView.Add(adtoReturn);
                }
                var list = adsToView.ToList();
                return list;
            }

        }

        public async Task<Promo> GetNotificationToReturn(ICollection<Promo> promos, string userId)
        {
            Random random = new Random();
            int startIndex = 0;
            if (userId != "")
            {
                var searchQuery = await GetBuyerSearchHistoryOfUser(userId);
                var pageViews = await GetPageViewHistoryOfUser(userId);
                IList<PromoScoreDto> promoScoreList = new List<PromoScoreDto>();

                if (promos.Count > 50)
                {
                    int randomGeneratorLimit = promos.Count / 50;
                    

                    startIndex = random.Next(randomGeneratorLimit) * 50;
                }

                for (int i = startIndex; i < promos.Count && i < startIndex + 50; i++)
                {
                    int searchQueryScore = 0;
                    int pageViewScore = 0;
                    var promo = promos.ElementAt(i);
                    var filter = _mapper.Map<PromoScoreDto>(promo);
                    var product = await _repo.GetProduct(promo.ProductId);

                    foreach (var record in searchQuery)
                    {
                        TimeSpan timeDiff = (DateTime.UtcNow - record.LatestVisit);
                        var time = Convert.ToInt32(timeDiff.TotalDays);
                        if (promo.Promotion_Name.Contains(record.Keyword))
                        {
                            searchQueryScore += ((2 * record.NoOfSearch) + (time < 7 ? 5 : 0));
                        }

                        if (promo.Promotion_Description.Contains(record.Keyword))
                        {
                            searchQueryScore += ((1 * record.NoOfSearch) + (time < 7 ? 5 : 0));
                        }

                        if (product.Product_name.Contains(record.Keyword))
                        {
                            searchQueryScore += ((2 * record.NoOfSearch) + (time < 7 ? 5 : 0));
                        }

                        if (product.Product_Discription.Contains(record.Keyword))
                        {
                            searchQueryScore += ((1 * record.NoOfSearch) + (time < 7 ? 5 : 0));
                        }

                    }

                    foreach (var record in pageViews)
                    {
                        TimeSpan timeDiff = (DateTime.UtcNow - record.LatestVisit);
                        var time = Convert.ToInt32(timeDiff.TotalDays);

                        if (product.Sub_category.Equals(record.Sub_category))
                        {
                            pageViewScore += ((3 * record.NoOfVisits) + (time < 7 ? 5 : 0));
                            break;
                        }
                    }
                
                    var finalScore = pageViewScore + searchQueryScore;
                    filter.Score = finalScore;
                    
                }

                var orderedPromo = promoScoreList.OrderByDescending(p => p.Score);
                var selectedPromo = orderedPromo.ElementAt(0);
                var returnPromo = _mapper.Map<Promo>(selectedPromo);
                return returnPromo;
            }else{

                var limit = promos.Count;
                var rand = random.Next(limit);
                return promos.ElementAt(rand);
            }
        }
    }
}