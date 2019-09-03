using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class AnalyticsService : iAnalytics
    {
        private readonly DataContext _context;

        public AnalyticsService(DataContext context)
        {
            _context = context;
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

        public async Task<BuyerSearch> GetBuyerSearchRecordIfAvailable(string Keyword, string UserId)
        {
            var data = await _context.BuyerSearch.FirstOrDefaultAsync(i => i.Keyword == Keyword && i.UserId == UserId);
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
    }
}