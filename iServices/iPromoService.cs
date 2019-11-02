using pro.backend.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pro.backend.iServices
{
    public interface iPromoService
    {

        Task<ICollection<Promo>> GetAllPromosOfSeller(string userId);

        Task<ICollection<Promo>> GetAllPromos();
        Task<ICollection<Promo>> GetAllActivePromosOfSeller(string userId);

        Task<ICollection<Promo>> GetAllActivePromosOfSellerOnSpecificDay(string userId,string dayOfTheWeek);

        Task<ICollection<Promo>> GetAllPendingPromosOfSeller(string userId);

        Task<ICollection<Promo>> GetAllPendingPromos();

        Task<ICollection<Promo>> GetAllActivePromos();

        Task<bool> UpdatePromo(Promo promo);

        Task<bool> UpdatePromoStatus(int promotionId,string status);

        Task<Promo> GetPromo(int Id);

    }
}