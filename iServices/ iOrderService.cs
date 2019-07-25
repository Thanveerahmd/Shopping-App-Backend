using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;

namespace pro.backend.iServices
{
    public interface  iOrderService
    {
        Task<ICollection<Order>> GetOrdersForBuyer(string Buyerid);
         Task<ICollection<orderDetails>> GetOrdersOfSeller(string Sellerid);
    }
}