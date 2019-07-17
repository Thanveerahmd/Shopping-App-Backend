using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class AdvertisementService : iAdvertisement
    {
        private readonly DataContext _context;

        public AdvertisementService(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Advertisement>> GetAcceptedAdvertisement()
        {
            var ad = await _context.Advertisement.Where(p => p.Status.ToLower().Equals("accepted")).ToListAsync();

            return ad;

        }

        public async Task<Advertisement> GetAdvertisement(int Id)
        {
            var ad = await _context.Advertisement.FirstOrDefaultAsync(p => p.Id == Id);

            return ad;

        }

        public async Task<ICollection<Advertisement>> GetAllAdvertisementOfSeller(string userId)
        {
            var ad = await _context.Advertisement.Where(p => p.UserId == userId).ToListAsync();

            return ad;
        }

        public string GetPaymentStatusOfAdvertisement(int id)
        {
            var ad = _context.Advertisement.FirstOrDefaultAsync(p => p.Id == id);
            return ad.Result.PaymentStatus;
        }

        public async Task<ICollection<Advertisement>> GetPendingAdvertisement()
        {
            var ad = await _context.Advertisement.Where(p => p.Status.ToLower().Equals("pending")).ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetRejectedAdvertisement()
        {
            var ad = await _context.Advertisement.Where(p => p.Status.ToLower().Equals("rejected")).ToListAsync();

            return ad;
        }

        public async Task<bool> UpdateAdvertisement(Advertisement ad)
        {
            var advertisement = await _context.Advertisement.FindAsync(ad.Id);
            if (advertisement == null)
                throw new AppException("advertisement not found");
            if(advertisement.Status!=null)
            advertisement.Status = ad.Status.ToLower();
            if(advertisement.PaymentStatus !=null)
            advertisement.PaymentStatus = ad.PaymentStatus.ToLower();
            advertisement.PublicID = ad.PublicID;
            advertisement.Url = ad.Url;
            advertisement.PhotoForAd = ad.PhotoForAd;

            _context.Advertisement.Update(advertisement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Advertisement> GetAd(int Id)
        {
            var ad = await _context.Advertisement.
                    Where(p => (p.Status.ToLower().Equals("accepted") || p.Status.ToLower().Equals("pending"))
                    && (p.PaymentStatus.ToLower().Equals("success") || p.PaymentStatus.ToLower().Equals("pending")))
                    .FirstOrDefaultAsync(p => p.ProductId == Id);

            return ad;
        }

        public async Task<ICollection<Advertisement>> ViewAdvertisement()
        {
            // YOU Have add timestamp Logic 
            var ad = await _context.Advertisement
                .Where(p => p.Status.ToLower().Equals("accepted") && p.PaymentStatus.ToLower().Equals("success"))
                .ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetActiveAdvertisementOfSeller(string sellerId)
        {
            var ad = await _context.Advertisement
                .Where(p => p.UserId == sellerId)
                .Where(p => p.Status.ToLower().Equals("accepted") && p.PaymentStatus.ToLower().Equals("success") &&p.ActivationStatus.ToLower().Equals("not expired"))
                .ToListAsync();

            return ad;
        }
        public async Task<ICollection<Advertisement>> GetAcceptedAdvertisementOfSeller(string sellerId)
        {
            var ad = await _context.Advertisement
                .Where(p => p.UserId == sellerId)
                .Where(p => p.Status.ToLower().Equals("accepted") && !p.PaymentStatus.ToLower().Equals("success"))
                .ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetPendingAdvertisementOfSeller(string sellerId)
        {
            var ad = await _context.Advertisement
                .Where(p => p.UserId == sellerId)
                .Where(p => p.Status.ToLower().Equals("pending") && p.ActivationStatus.ToLower().Equals("not expired"))
                .ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetExpiredAdvertisementOfSeller(string sellerId)
        {
            var ad = await _context.Advertisement
                .Where(p => p.UserId == sellerId)
                .Where(p => p.ActivationStatus.ToLower().Equals("expired"))
                .ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetRejectedAdvertisementOfSeller(string sellerId)
        {
            var ad = await _context.Advertisement
                .Where(p => p.UserId == sellerId)
                .Where(p => p.Status.ToLower().Equals("rejected"))
                .ToListAsync();

            return ad;
        }

    }
}