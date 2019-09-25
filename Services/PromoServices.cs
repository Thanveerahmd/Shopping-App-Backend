using pro.backend.Entities;
using Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.iServices;
using System;

namespace pro.backend.Services
{
    public class PromoService : iPromoService
    {
        private DataContext _context;

        public PromoService(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Promo>> GetAllPromosOfSeller(string userId)
        {
            var promo = await _context.Promo
            .Where(p => p.UserId == userId)
            .ToListAsync();

            return promo;
        }

        public async Task<ICollection<Promo>> GetAllPromos()
        {
            var promo = await _context.Promo
            .ToListAsync();

            return promo;
        }

        public async Task<ICollection<Promo>> GetAllPendingPromos()
        {
            var promo = await _context.Promo
            .Where(p => p.Status.ToLower().Equals("pending"))
            .ToListAsync();

            return promo;
        }

        public async Task<ICollection<Promo>> GetAllActivePromos()
        {
            var promo = await _context.Promo
            .Where(p => p.Status.ToLower().Equals("accepted"))
            .ToListAsync();

            return promo;
        }

        public async Task<ICollection<Promo>> GetAllActivePromosOfSeller(string userId)
        {
            var promo = await _context.Promo
            .Where(p => p.UserId == userId && p.Status.ToLower().Equals("accepted"))
            .ToListAsync();

            return promo;
        }

        public async Task<ICollection<Promo>> GetAllPendingPromosOfSeller(string userId)
        {
            var promo = await _context.Promo
            .Where(p => p.UserId == userId && p.Status.ToLower().Equals("pending"))
            .ToListAsync();

            return promo;
        }


        public async Task<bool> UpdatePromo(Promo promo)
        {
            var promotion = await _context.Promo.FindAsync(promo.Id);
            if (promotion == null)
                throw new AppException("advertisement not found");

            if (promotion.Status != null)
                promotion.Status = promo.Status.ToLower();
            promotion.ExpiryDate = promo.ExpiryDate;
            promotion.Promotion_Description = promo.Promotion_Description;
            promotion.Promotion_Name = promo.Promotion_Name;
            promotion.Frequency = promo.Frequency;
            promotion.Day_of_The_Week = promo.Day_of_The_Week;

            _context.Promo.Update(promotion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePromoStatus(int promotionId, string status)
        {
            var promotion = await _context.Promo.FindAsync(promotionId);
            if (promotion == null)
                throw new AppException("promotion not found");

            if (promotion.Status != null)
            {
                promotion.Status = status.ToLower();

                if (promotion.Status.Equals("accepted"))
                {
                    promotion.ExpiryDate = DateTime.Now.AddDays(7 * promotion.Frequency);
                }

            }


            _context.Promo.Update(promotion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Promo> GetPromo(int Id)
        {
            var ad = await _context.Promo.FirstOrDefaultAsync(p => p.Id == Id);

            return ad;

        }
    }
}