using System.Collections.Generic;
using System.Threading.Tasks;
using pro.backend.Entities;
using System.Linq;
using pro.backend.Dtos;

namespace pro.backend.iServices
{
    public interface iOrderService
    {
        Task<IOrderedEnumerable<Order>> GetOrdersForBuyer(string Buyerid);
        Task<IOrderedEnumerable<orderDetails>> GetOrdersOfSeller(string Sellerid);
        Task<IOrderedEnumerable<Order>> GetOrders();
        Task<Order> GetOrderById(int orderId);
        Task<bool> UpdateOrderDeliveryStatus(int orderId);
        ICollection<OrderDetailsForUserPreference> GetAllOrderDetails();

    }
}