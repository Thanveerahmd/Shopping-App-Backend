using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface iAdvertisement
    {
        Task<ICollection<Advertisement>> GetPendingAdvertisement();
        Task<ICollection<Advertisement>> GetAcceptedAdvertisement();
         Task<ICollection<Advertisement>> GetAllAdvertisementOfSeller(string userId);
        Task<ICollection<Advertisement>> GetRejectedAdvertisement();
        string GetPaymentStatusOfAdvertisement(int id);
        Task<bool> UpdateAdvertisementStatus(Advertisement ad);
         Task<Advertisement> GetAdvertisement(int Id);


    }
}