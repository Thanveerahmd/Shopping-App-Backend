using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class MapService : iMapService
    {
        
        private readonly DataContext _context;

        public MapService(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Store>> GetAllStores()
        {
            var info = await _context.Store.ToListAsync();

            return info;
        }

        public async Task<ICollection<Store>> GetAllStoresOfSeller(string sellerID)
        {
            var info = await _context.Store.Where(p => p.UserId == sellerID).ToListAsync();

            return info;
        }

        public async Task<Store> GetStore(int id)
        {
            var Store = await _context.Store.FindAsync(id);
            return Store;
        }

        public async Task<bool> UpdateStore(Store Store)
        {
             var Storeinfo = await _context.Store.FindAsync(Store.Id);
            if (Storeinfo == null)
                throw new AppException("Store is not avilable ");

            Storeinfo.StoreName = Store.StoreName;
            Storeinfo.lat=Store.lat;
            Storeinfo.lng=Store.lng;

             _context.Store.Update(Storeinfo);
            return await _context.SaveChangesAsync()>0;
        }
    }
}