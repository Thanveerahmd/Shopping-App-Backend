using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface iMapService
    {
        Task<Store> GetStore(int id);
        Task<ICollection<Store>> GetAllStoresOfSeller(string sellerID);
        Task<ICollection<Store>> GetAllStores();
        Task<bool> UpdateStore(Store Store);
        Task<bool> LocationUpdate(DeviceToken location);
        Task<DeviceToken> GetDeviceDetails(string deviceId);
    }
}