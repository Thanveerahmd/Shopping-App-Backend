using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;
using Project.Entities;
using Project.Helpers;
using Project.Services;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("Purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        public iProductService _ProductService;
        public IEmailSender _emailSender;
        private readonly UserManager<User> _usermanger;
        private readonly iAdvertisement _adService;

        public PurchaseController(
        IMapper mapper,
        iShoppingRepo repo,
        iProductService ProductService,
        IEmailSender EmailSender,
         UserManager<User> usermanger,
         iAdvertisement AdService)
        {
            _mapper = mapper;
            _repo = repo;
            _ProductService = ProductService;
            _emailSender = EmailSender;
            _usermanger = usermanger;
            _adService = AdService;
        }

        [HttpPost("PaymentForOrders")]
        [AllowAnonymous]
        public async void PaymentDetails([FromBody]PaymentInfoDto PaymentInfoDto)
        {
            var paymentInfo = _mapper.Map<BuyerPaymentInfo>(PaymentInfoDto);

            if (paymentInfo.status_code == 2)
            {
                var order = await _repo.GetOrder(paymentInfo.order_id);
                order.PaymentStatus = "success";
                var OrderProducts = order.orderDetails;
                order.Total_Price = paymentInfo.payhere_amount;

                foreach (var Orderproduct in OrderProducts)
                {
                    var product = await _repo.GetProduct(Orderproduct.ProductId);
                    var oldQuntity = product.Quantity;
                    product.Quantity = (oldQuntity - Orderproduct.Count);

                    if (product.Quantity <= product.ReorderLevel)
                    {
                        var seller = await _usermanger.FindByIdAsync(product.SellerId);
                        await _emailSender.SendEmailAsync(seller.UserName, "About ReOrderLevel ",
                     $"Your product " + product.Product_name + " has reached to the ReOrder Level ");
                    }
                    try
                    {
                        await _ProductService.UpdateProduct(product);
                    }
                    catch (AppException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                try
                {
                    await _repo.UpdateOrder(order);
                    _repo.Add(paymentInfo);
                }
                catch (AppException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (await _repo.SaveAll())
                {
                    Console.WriteLine("Success");
                }
            }
            else if (paymentInfo.status_code == -1 || paymentInfo.status_code == -2)
            {
                var order = await _repo.GetOrder(paymentInfo.order_id);
                order.PaymentStatus = "canceled Or Failed";
                order.Total_Price = 0;
                try
                {
                    await _repo.UpdateOrder(order);
                    Console.WriteLine("canceled Or Failed");
                }
                catch (AppException ex)
                {

                    Console.WriteLine(ex.Message);

                }
            }
            else if (paymentInfo.status_code == -3)
            {
                var order = await _repo.GetOrder(paymentInfo.order_id);
                order.PaymentStatus = "chargedback";
                var OrderProducts = order.orderDetails;
                order.Total_Price = paymentInfo.payhere_amount;

                foreach (var Orderproduct in OrderProducts)
                {
                    var product = await _repo.GetProduct(Orderproduct.ProductId);
                    var oldQuntity = product.Quantity;
                    product.Quantity = (oldQuntity + Orderproduct.Count);

                    try
                    {
                        await _ProductService.UpdateProduct(product);
                    }
                    catch (AppException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                try
                {
                    await _repo.UpdateOrder(order);
                    _repo.Add(paymentInfo);
                }
                catch (AppException ex)
                {

                    Console.WriteLine(ex.Message);

                }

                if (await _repo.SaveAll())
                {
                    Console.WriteLine("chargedback");

                }

            }
            else
            {
                Console.WriteLine("pending");
            }
        }


        // need to do testing 
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


        [HttpPost("PaymentForAdvertisement")]
        [AllowAnonymous]
        public async void PaymentDetailsofAdvertisements([FromBody]PaymentInfoDto PaymentInfoDto)
        {

            var paymentInfo = _mapper.Map<SellerPaymentInfo>(PaymentInfoDto);
            var ad = await _adService.GetAdvertisement(paymentInfo.order_id);

            if (paymentInfo.status_code == 2)
            {
                ad.PaymentStatus = "";
            }
            else if (paymentInfo.status_code == -1 || paymentInfo.status_code == -2)
            {

            }
            else if (paymentInfo.status_code == -3)
            {

            }
            else
            {
                Console.WriteLine("pending");
            }

        }


       
    }
}