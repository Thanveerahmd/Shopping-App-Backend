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

        public async Task<IOrderedEnumerable<Order>> GetOrdersForBuyer(string Buyerid)
        {
            var orders = await _context.Orders
            .Where(p => p.BuyerId == Buyerid)
            .Include(p => p.orderDetails)
            .ToListAsync();

            var data = orders.OrderByDescending(p => p.Id);
            return data;
        }

        public async Task<IOrderedEnumerable<orderDetails>> GetOrdersOfSeller(string Sellerid)
        {
            var orders = await _context.orderDetails.FromSql("Select * from orderDetails where orderDetails.OrderId in (Select Id from Orders where Orders.PaymentStatus = 'success') and orderDetails.sellerId='"+Sellerid+"'")
            .ToListAsync();
            //  var orders = await _context.Orders.FromSql("Select * from orderDetails JOIN Orders on(Orders.Id = orderDetails.OrderId) where orderDetails.OrderId in (Select Id from Orders where Orders.PaymentStatus = 'success') and orderDetails.sellerId='"+Sellerid+"'")
            // .ToListAsync();
            // string sql = "Select OD.Id,OD.ProductId,OD.product_Name,OD.MainPhotoUrl,OD.Price,OD.Count,OD.OrderId,OD.sellerId,O.BuyerId,O.DateAdded,O.PaymentStatus,O.Total_Price,O.DeliveryInfoId,D.FName,D.Address,D.District,D.City,D.MobileNumber from orderDetails OD, Orders O ,DeliveryInfo D where O.Id = OD.OrderId AND D.Id=O.DeliveryInfoId AND OD.OrderId in (Select Id from Orders where O.PaymentStatus = 'success') and OD.sellerId='"+Sellerid+"'";

            // var orders = await _context.Orders.FromSql(sql)
            // .ToListAsync();
            var data = orders.OrderByDescending(p => p.Id);
            return data;
        }
    }
}