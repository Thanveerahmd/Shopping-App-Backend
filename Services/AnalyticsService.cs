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

namespace pro.backend.Services
{
    public class AnalyticsService : iAnalytics
    {
        private readonly DataContext _context;
        private iOrderService _order;
        public readonly iShoppingRepo _repo;



        public AnalyticsService(DataContext context, iOrderService order,iShoppingRepo repo)
        {
            _context = context;
            _order = order;
            _repo = repo;

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

        IDataModel LoadOrdersDataModel()
        {

            IDataModel dataModel = null;
            dataModel = Load();
            return dataModel;
        }


        IDataModel GetDataModelForNewUser(IDataModel baseModel, params long[] preferredItems)
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



        IList<Product> iAnalytics.getRecommendation(int currentProductID)
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
    }
}