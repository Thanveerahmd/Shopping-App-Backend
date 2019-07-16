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
            var ad =await _context.Advertisement.Where(p => p.Status.Equals("Accepted")).ToListAsync();

            return ad;

        }

        public async Task<Advertisement> GetAdvertisement(int Id)
        {
            var ad =await _context.Advertisement.FirstOrDefaultAsync(p => p.Id ==Id);

            return ad;

        }

        public async Task<ICollection<Advertisement>> GetAllAdvertisementOfSeller(string userId)
        {
           var ad =await _context.Advertisement.Where(p =>p.UserId == userId).ToListAsync();

            return ad;
        }

        public string GetPaymentStatusOfAdvertisement(int id)
        {
            var ad = _context.Advertisement.FirstOrDefaultAsync(p=> p.Id == id);
            return ad.Result.PaymentStatus;
        }

        public async Task<ICollection<Advertisement>> GetPendingAdvertisement()
        {
            var ad =await _context.Advertisement.Where(p => p.Status.Equals("Pending") ).ToListAsync();

            return ad;
        }

        public async Task<ICollection<Advertisement>> GetRejectedAdvertisement()
        {
             var ad =await _context.Advertisement.Where(p => p.Status.Equals("Rejected") ).ToListAsync();

            return ad;
        }

        public async Task<bool> UpdateAdvertisementStatus(Advertisement ad)
        {
            var  advertisement = await _context.Advertisement.FindAsync(ad.Id);
            if (advertisement == null)
                throw new AppException("advertisement not found");

            advertisement.Status = ad.Status;

            _context.Advertisement.Update(advertisement);
            return await _context.SaveChangesAsync()>0;
        }
    }
}