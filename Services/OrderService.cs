using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class OrderService : iOrderService
    {
        private readonly DataContext _context;
        public readonly iShoppingRepo _repo;

        public OrderService(DataContext context, iShoppingRepo repo)
        {
            _context = context;
            _repo = repo;
        }

        public async Task<ICollection<Order>> GetOrdersForBuyer(string Buyerid)
        {
            var orders = await _context.Orders
            .Where(p => p.BuyerId == Buyerid)
            .Include(p => p.orderDetails)
            .ToListAsync();

            return orders;
        }

        public async Task<ICollection<orderDetails>> GetOrdersOfSeller(string Sellerid)
        {
            var orders = await _context.orderDetails.FromSql("Select * from orderDetails where orderDetails.OrderId in (Select Id from Orders where Orders.PaymentStatus = 'success') and orderDetails.sellerId='"+Sellerid+"'")
            .ToListAsync();

            return orders;
        }
    }
}