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

        [HttpGet("seller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOrdersOfSeller(string SellerId)
        {
            var order = await _order.GetOrdersOfSeller(SellerId);
            
            return Ok(order);
        }

    }
}