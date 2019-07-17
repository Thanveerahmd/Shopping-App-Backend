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
    [Route("Ad")]
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

        [HttpPost("Advertisement/{SellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAdvertisements([FromBody]AdvertismentUploadDto adDto)
        {
            var ad = _mapper.Map<Advertisement>(adDto);

            if ((await _adService.GetAd(ad.ProductId)) == null)
            {
                ad.PaymentStatus = "Pending";
                _repo.Add(ad);

                if (await _repo.SaveAll())
                {
                    return Ok(ad.Id);
                }
                return BadRequest("");
            }

            return BadRequest("This product alread have a Advertisement");
        }


        [HttpGet("AcceptedAdvertisements")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewAdvertisement()
        {
            var ad = await _adService.ViewAdvertisement();
            return Ok(ad);
        }

    }
}