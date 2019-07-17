using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;
using Project.Helpers;
using Project.Services;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("adverts")]
    public class AdController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        public iProductService _ProductService;
        public IEmailSender _emailSender;
        private readonly iAdvertisement _adService;

        public AdController(
        IMapper mapper,
        iShoppingRepo repo,
        iProductService ProductService,
        IEmailSender EmailSender,
         iAdvertisement AdService)
        {
            _mapper = mapper;
            _repo = repo;
            _ProductService = ProductService;
            _emailSender = EmailSender;
            _adService = AdService;
        }

        [HttpPost("{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAdvertisements([FromBody]AdvertismentUploadDto adDto, string SellerId)
        {
            var ad = _mapper.Map<Advertisement>(adDto);
            var prevAd = await _adService.GetAd(ad.ProductId);
            if (prevAd == null)
            {
                ad.PaymentStatus = "pending";
                ad.Status ="pending";
                ad.ActivationStatus = "not expired";
                ad.UserId = SellerId;
                _repo.Add(ad);

                if (await _repo.SaveAll())
                {
                    return Ok(ad.Id);
                }
                return BadRequest(new { message = "Advertisement not saved" });
            }

            return BadRequest(new { message = "This product already has an Advertisement" });
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ViewAdvertisement()
        {
            var ad = await _adService.ViewAdvertisement();

            IList<AdvertisementToReturnDto> adsToView = new List<AdvertisementToReturnDto>();

            foreach (var item in ad)
            {
                var adtoReturn = _mapper.Map<AdvertisementToReturnDto>(item);
                var product = await _repo.GetProduct(item.ProductId);
                adtoReturn.ProductName = product.Product_name;
                adtoReturn.ProductPrice = product.Price;
                adsToView.Add(adtoReturn);
            }

            return Ok(adsToView);
        }

        [HttpDelete("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Deleteinfo(int Id)
        {

            var ad = await _adService.GetAdvertisement(Id);

            _repo.Delete(ad);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest(new{message = "Could Not Delete Ad"});

        }

        [HttpGet("acceptedBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAcceptedAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetAcceptedAdvertisementOfSeller(SellerId);
            IList<AdvertisementToReturnDto> adsToView = new List<AdvertisementToReturnDto>();

            foreach (var item in ad)
            {
                var adtoReturn = _mapper.Map<AdvertisementToReturnDto>(item);
                var product = await _repo.GetProduct(item.ProductId);
                adtoReturn.ProductName = product.Product_name;
                adtoReturn.ProductPrice = product.Price;
                adsToView.Add(adtoReturn);
            }

            return Ok(adsToView);
        }

        [HttpGet("activeBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetActiveAdvertisementOfSeller(SellerId);
            IList<AdvertisementToReturnDto> adsToView = new List<AdvertisementToReturnDto>();

            foreach (var item in ad)
            {
                var adtoReturn = _mapper.Map<AdvertisementToReturnDto>(item);
                var product = await _repo.GetProduct(item.ProductId);
                adtoReturn.ProductName = product.Product_name;
                adtoReturn.ProductPrice = product.Price;
                adsToView.Add(adtoReturn);
            }

            return Ok(adsToView);
        }

        [HttpGet("pendingBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPendingAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetPendingAdvertisementOfSeller(SellerId);
            IList<AdvertisementToReturnDto> adsToView = new List<AdvertisementToReturnDto>();

            foreach (var item in ad)
            {
                var adtoReturn = _mapper.Map<AdvertisementToReturnDto>(item);
                var product = await _repo.GetProduct(item.ProductId);
                adtoReturn.ProductName = product.Product_name;
                adtoReturn.ProductPrice = product.Price;
                adsToView.Add(adtoReturn);
            }

            return Ok(adsToView);
        }

        [HttpGet("expiredBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExpiredAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetExpiredAdvertisementOfSeller(SellerId);
            IList<AdvertisementToReturnDto> adsToView = new List<AdvertisementToReturnDto>();

            foreach (var item in ad)
            {
                var adtoReturn = _mapper.Map<AdvertisementToReturnDto>(item);
                var product = await _repo.GetProduct(item.ProductId);
                adtoReturn.ProductName = product.Product_name;
                adtoReturn.ProductPrice = product.Price;
                adsToView.Add(adtoReturn);
            }

            return Ok(adsToView);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateAd(AdvertisementToReturnDto adDto)
        {
            var ad = _mapper.Map<Advertisement>(adDto);
            
            ad.Id = adDto.Id;
            ad.DateAdded = DateTime.Now;

            try
            {
                // save 
                await _adService.UpdateAdvertisement(ad);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}