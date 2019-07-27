using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;
using System.Linq;

namespace pro.backend.iServices
{
    public interface  iOrderService
    {
        Task<IOrderedEnumerable<Order>> GetOrdersForBuyer(string Buyerid);
         Task<IOrderedEnumerable<orderDetails>> GetOrdersOfSeller(string Sellerid);
    }
}