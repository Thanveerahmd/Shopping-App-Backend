using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;
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
        public async Task<IActionResult> AddAdvertisements([FromBody]AdvertismentUploadDto adDto)
        {
            var ad = _mapper.Map<Advertisement>(adDto);

            if ((await _adService.GetAd(ad.ProductId)) == null)
            {
                ad.PaymentStatus = "pending";
                _repo.Add(ad);

                if (await _repo.SaveAll())
                {
                    return Ok(ad.Id);
                }
                return BadRequest(new { message = "Advertisement not saved" });
            }

            return BadRequest( new {message = "This product already has an Advertisement"});
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ViewAdvertisement()
        {
            var ad = await _adService.ViewAdvertisement();
            return Ok(ad);
        }

        [HttpDelete("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Deleteinfo(int Id)
        {

            var ad =await _adService.GetAdvertisement(Id);
           
            _repo.Delete(ad);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();

        }

        [HttpGet("acceptedBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAcceptedAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetAcceptedAdvertisementOfSeller(SellerId);
            return Ok(ad);
        }

        [HttpGet("activeBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetActiveAdvertisementOfSeller(SellerId);
            return Ok(ad);
        }

        [HttpGet("pendingBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPendingAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetPendingAdvertisementOfSeller(SellerId);
            return Ok(ad);
        }

        [HttpGet("expiredBySeller/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExpiredAdvertisementOfSeller(string SellerId)
        {
            var ad = await _adService.GetExpiredAdvertisementOfSeller(SellerId);
            return Ok(ad);
        }

    }
}