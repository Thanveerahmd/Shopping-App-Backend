using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("Purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        public PurchaseController(
        IMapper mapper, iShoppingRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // [HttpPost]
        // [AllowAnonymous]
        // public IActionResult AddPaymentDetails([FromBody]PaymentInfoDto PaymentInfoDto)
        // {

        //     // map dto to entity
        //    // var PaymentInfo = _mapper.Map<>(PaymentInfoDto);
        //     try
        //     {
        //         _productService.AddProduct(product);
        //         return Ok(product.Id);

        //     }
        //     catch (AppException ex)
        //     {

        //         return BadRequest(new { message = ex.Message });
        //     }

        // }

        [HttpPost("checkout/{BuyerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> checkout(checkoutDto checkoutDto, string BuyerId)
        {
            var order = new Order();
            order.BuyerId = BuyerId;
            order.PaymentStatus = "Pending";
            order.DateAdded = DateTime.Now;

            if (checkoutDto.CartId == 0)
            {
                // should product quntity be less ???
                // relation between product id and specification

                var BuyNowProduct = _mapper.Map<orderDetails>(checkoutDto);
                var product = _repo.GetProduct(BuyNowProduct.ProductId).Result;

                if (product.Quantity < BuyNowProduct.Count)
                    return BadRequest(BuyNowProduct.product_Name);

                _repo.Add(order);


                if (await _repo.SaveAll())
                {
                    var ordertab = _repo.GetOrder(order.Id).Result;
                    ordertab.orderDetails.Add(BuyNowProduct);

                    if (await _repo.SaveAll())
                    {
                        return Ok(order.Id);
                    }
                    return BadRequest("OrderDetail is not added");
                }
                return BadRequest("OrderDetail is not added");




            }
            else
            {
                var orderDetails = _repo.GetAllCartProduct(checkoutDto.CartId).Result;
                var counter = 0;

                IList<string> OutOfStockProducts = new List<string>();

                foreach (var el in orderDetails)
                {
                    var CartProduct = _mapper.Map<OrderProductDto>(el);
                    var OrderProduct = _mapper.Map<orderDetails>(CartProduct);
                    var product = _repo.GetProduct(CartProduct.ProductId).Result;

                    if (product.Quantity < CartProduct.Count)
                    {
                        counter++;
                        OutOfStockProducts.Add(OrderProduct.product_Name);
                    }
                }

                if (counter > 0)
                {
                    return BadRequest(OutOfStockProducts);
                }


                try
                {
                    _repo.AddOrder(order);
                    var ordertab = _repo.GetOrder(order.Id).Result;
                    foreach (var el in orderDetails)
                    {
                        var CartProduct = _mapper.Map<OrderProductDto>(el);
                        var OrderProduct = _mapper.Map<orderDetails>(CartProduct);
                        ordertab.orderDetails.Add(OrderProduct);
                    }
                      _repo.DeleteAll(orderDetails);

                    if (await _repo.SaveAll())
                    {
                        return Ok(order.Id);
                    }
                    return BadRequest();
                }
                catch (AppException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

            }
        }

    }
}