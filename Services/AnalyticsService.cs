using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;
using AutoMapper;
using pro.backend.Dtos;

namespace pro.backend.Services
{
    public class AnalyticsService : iAnalytics
    {
        private readonly DataContext _context;

        private readonly IMapper _mapper;

        public readonly iShoppingRepo _repo;

        public AnalyticsService(
            DataContext context,
            iShoppingRepo repo,
            IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
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
                        filteredAdverts.Add(filter);
                    }

                }

                var searchQuery = await GetBuyerSearchHistoryOfUser(userId);
                var pageViews = await GetPageViewHistoryOfUser(userId);

                foreach (var record in filteredAdverts)
                {
                    int searchQueryScore = 0;
                    int pageViewScore = 0;
                    foreach (var value in searchQuery)
                    {
                        if (record.ProductName != null && record.ProductDescription != null)
                        {
                            TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                            var time = Convert.ToInt32(timeDiff.TotalDays);
                            if (record.ProductName.Contains(value.Keyword))
                            {
                                searchQueryScore += ((2 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                            }

                            if (record.ProductDescription.Contains(value.Keyword))
                            {
                                searchQueryScore += ((1 * value.NoOfSearch) + (time < 7 ? 5 : 0));
                            }
                        }


                    }

                    foreach (var value in pageViews)
                    {
                        if (record.ProductName != null && record.ProductDescription != null)
                        {
                            TimeSpan timeDiff = (DateTime.UtcNow - value.LatestVisit);
                            var time = Convert.ToInt32(timeDiff.TotalDays);
                            if (record.ProductSubCategory.Equals(value.Sub_category))
                            {
                                pageViewScore += ((3 * value.NoOfVisits) + (time < 7 ? 5 : 0));
                            }
                        }

                    }
                    record.Score = searchQueryScore + pageViewScore;
                }

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

    }
}