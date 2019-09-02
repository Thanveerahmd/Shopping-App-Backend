using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface iAdvertisement
    {
        Task<ICollection<Advertisement>> GetPendingAdvertisement();
        Task<ICollection<Advertisement>> GetActiveAdvertisement();
        Task<ICollection<Advertisement>> GetAllAdvertisementOfSeller(string userId);
        Task<ICollection<Advertisement>> GetRejectedAdvertisement();
        string GetPaymentStatusOfAdvertisement(int id);
        Task<bool> UpdateAdvertisement(Advertisement ad);
        Task<Advertisement> GetAdvertisement(int Id);
        Task<Advertisement> GetAd(int Id);
        Task<PhotoForAd> GetPhotoOfad(int Id);
        Task<ICollection<Advertisement>> ViewAdvertisement();
        Task<ICollection<Advertisement>> GetActiveAdvertisementOfSeller(string sellerId);
        Task<ICollection<Advertisement>> GetAcceptedAdvertisementOfSeller(string sellerId);
        Task<ICollection<Advertisement>> GetPendingAdvertisementOfSeller(string sellerId);
        Task<ICollection<Advertisement>> GetExpiredAdvertisementOfSeller(string sellerId);
        Task<ICollection<Advertisement>> GetRejectedAdvertisementOfSeller(string sellerId);
        Task<ICollection<Advertisement>> GetExpiredAdvertisement();
    }
}