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
    [Route("purchase")]
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

        [HttpPost("paymentForOrders")]
        [AllowAnonymous]
        public async Task PaymentDetails([FromBody]PaymentInfoDto PaymentInfoDto)
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
                    Console.WriteLine("success");
                }
            }
            else if (paymentInfo.status_code == -1 || paymentInfo.status_code == -2)
            {
                var order = await _repo.GetOrder(paymentInfo.order_id);

                if (paymentInfo.status_code == -1)
                {
                    order.PaymentStatus = "canceled";
                }
                else
                {
                    order.PaymentStatus = "failed";
                    var Buyer = await _usermanger.FindByIdAsync(order.BuyerId);
                    await _emailSender.SendEmailAsync(Buyer.UserName, "About Payment Done on your Order",
                 $"Your payment status is failed  ");
                }

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
            order.PaymentStatus = "pending";
            order.DateAdded = DateTime.Now;

            if (checkoutDto.CartId == 0)
            {
                // should product quntity be less ???
                // relation between product id and specification

                var BuyNowProduct = _mapper.Map<orderDetails>(checkoutDto);
                var product = await _repo.GetProduct(BuyNowProduct.ProductId);

                if (product.Quantity < BuyNowProduct.Count)
                    return BadRequest(BuyNowProduct.product_Name);

                _repo.Add(order);


                if (await _repo.SaveAll())
                {
                    var ordertab = await _repo.GetOrder(order.Id);
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
                var orderDetails = await _repo.GetAllCartProduct(checkoutDto.CartId);
                var counter = 0;

                IList<string> OutOfStockProducts = new List<string>();

                foreach (var el in orderDetails)
                {
                    var CartProduct = _mapper.Map<OrderProductDto>(el);
                    var OrderProduct = _mapper.Map<orderDetails>(CartProduct);
                    var product = await _repo.GetProduct(CartProduct.ProductId);

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
                    var ordertab = await _repo.GetOrder(order.Id);
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


        [HttpPost("paymentForAdvertisement")]
        [AllowAnonymous]
        public async  Task PaymentDetailsofAdvertisements([FromBody]PaymentInfoDto PaymentInfoDto)
        {
            
                var paymentInfo = _mapper.Map<SellerPaymentInfo>(PaymentInfoDto);
                var ad = await _adService.GetAdvertisement(paymentInfo.order_id);
                var seller = await _usermanger.FindByIdAsync(ad.UserId);
                paymentInfo.UserId = seller.Id;


                if (paymentInfo.status_code == 2)
                {
                    ad.PaymentStatus = "success";
                    await _emailSender.SendEmailAsync(seller.UserName, $"Payment for Advert {paymentInfo.order_id}",
                        $"Your payment has been successful. Your payment Id is {PaymentInfoDto.payment_id}. Your Ad will be live for {ad.timestamp} days");
                    try
                    {
                        _repo.Add(paymentInfo);
                        await _adService.UpdateAdvertisement(ad);
                      //  return Ok();
                    }
                    catch (AppException ex)
                    {

                       // return BadRequest(ex.Message.ToString());
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (paymentInfo.status_code == -1 || paymentInfo.status_code == -2)
                {
                    if (paymentInfo.status_code == -1)
                    {
                        ad.PaymentStatus = "canceled";
                    }
                    else
                    {
                        ad.PaymentStatus = "failed";

                        await _emailSender.SendEmailAsync(seller.UserName, $"Payment for Advert {paymentInfo.order_id}",
                        $"Your payment has failed.");
                    }

                    try
                    {
                        _repo.Add(paymentInfo);
                        await _adService.UpdateAdvertisement(ad);
                       // return Ok();
                    }
                    catch (AppException ex)
                    {
                        //return BadRequest(ex.Message.ToString());
                         Console.WriteLine(ex.Message);
                    }
                }
                else if (paymentInfo.status_code == -3)
                {
                    ad.PaymentStatus = "chargedback";
                    await _emailSender.SendEmailAsync(seller.UserName, $"Payment ChargeBack for Advert {paymentInfo.order_id}",
                        $"Your payment has been chargebacked.");
                    try
                    {
                        await _repo.UpdateSellerInfo(paymentInfo);
                        await _adService.UpdateAdvertisement(ad);
                        // return Ok();

                    }
                    catch (AppException ex)
                    {

                        // return BadRequest(ex.Message.ToString());
                         Console.WriteLine(ex.Message);
                    }

                }
                else
                {
                    //return Ok();
                    Console.WriteLine("pending");
                }

           

        }



    }
}