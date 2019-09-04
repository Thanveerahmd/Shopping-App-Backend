using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class OrderService : iOrderService
    {
        private readonly DataContext _context;
        public readonly iShoppingRepo _repo;
        private readonly IMapper _mapper;

        public OrderService(DataContext context, iShoppingRepo repo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;

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
            var orders = await _context.orderDetails.FromSql("Select * from orderDetails where orderDetails.OrderId in (Select Id from Orders where Orders.PaymentStatus = 'success') and orderDetails.sellerId='" + Sellerid + "'")
            .ToListAsync();
            var data = orders.OrderByDescending(p => p.Id);
            return data;
        }

        public async Task<IOrderedEnumerable<Order>> GetOrders()
        {
            var orders = await _context.Orders
            .Include(p => p.orderDetails)
            .ToListAsync();

            var data = orders.OrderByDescending(p => p.Id);
            return data;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(p => p.Id == orderId);
            return order;
        }

        public async Task<bool> UpdateOrderDeliveryStatus(int orderId)
        {
            var OldOrder = await GetOrderById(orderId);

            if (OldOrder == null)
                return false;
            OldOrder.DeliveryStatus = true;

            _context.Orders.Update(OldOrder);
            return await _context.SaveChangesAsync() > 0;

        }

        public ICollection<OrderDetailsForUserPreference> GetAllOrderDetails()
        {
            var orders = _context.orderDetails.ToList();

            IList<OrderDetailsForUserPreference> OrderList = new List<OrderDetailsForUserPreference>();

            foreach (var item in orders)
            {
               var list = _mapper.Map<OrderDetailsForUserPreference>(item);
               list.BuyerId = GetOrderById(list.OrderId).Result.BuyerId;
               OrderList.Add(list);
            }
            return OrderList;
        }



    }
}