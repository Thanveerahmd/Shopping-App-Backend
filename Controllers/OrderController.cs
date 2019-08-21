using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Dtos;
using System.Collections.Generic;


namespace pro.backend.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        private iOrderService _order;

        public OrderController(
                IMapper mapper,
                iShoppingRepo repo,
                iOrderService order
                )
        {
            _mapper = mapper;
            _repo = repo;
            _order = order;
        }

        [HttpGet("buyer/{BuyerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOrdersOfBuyer(string BuyerId)
        {
            var order = await _order.GetOrdersForBuyer(BuyerId);
            var orderToReturn = _mapper.Map<ICollection<OrderReturnDto>>(order);
            return Ok(orderToReturn);
        }

        [HttpPut("deliveryStatus/{orderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDeliveryStatus(int orderId)
        {
          if(await _order.UpdateOrderDeliveryStatus(orderId))
              return Ok();

          return BadRequest();
        }

        [HttpGet("orders")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOrders()
        {
            var order = await _order.GetOrders();
            var orderToReturn = _mapper.Map<ICollection<OrderReturnDto>>(order);
            return Ok(orderToReturn);
        }

        [HttpGet("seller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOrdersOfSeller(string SellerId)
        {
            var orderdetail = await _order.GetOrdersOfSeller(SellerId);
            IList<OrderInfoForSellerDto> orderdetails = new List<OrderInfoForSellerDto>();

            foreach (var item in orderdetail)
            {
                var orderToReturn = _mapper.Map<OrderInfoForSellerDto>(item);
                var order = await _repo.GetOrder(orderToReturn.OrderId);
                var delivaryInfo = await _repo.GetDeliveryInfo(order.deliveyId);
                orderToReturn.deliveryInfo = delivaryInfo;
                var billingInfo = await _repo.GetBillingInfoOfDefault(order.BuyerId);
                orderToReturn.DeliveryStatus = order.DeliveryStatus;
                orderToReturn.emergencyContact = billingInfo.MobileNumber;
                orderdetails.Add(orderToReturn);
            }
            return Ok(orderdetails);
        }

    }
}