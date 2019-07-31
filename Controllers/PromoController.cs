using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.Helpers;
using pro.backend.iServices;
using Project.Entities;
using Project.Helpers;


namespace pro.backend.Controllers
{
    [ApiController]
    [Route("promo")]

    public class PromoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _usermanger;
        private readonly iPromoService _promoService;
        public readonly iShoppingRepo _repo;


        public PromoController(
            IMapper mapper,
            iPromoService promoService,
            iShoppingRepo repo,
            UserManager<User> usermanger
            )
        {

            _mapper = mapper;
            _promoService = promoService;
            _repo = repo;
            _usermanger = usermanger;

        }

        [HttpPost("user/add/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> addPromotion(string userId,PromoDto promo){
            var promotion = _mapper.Map<Promo>(promo);
            promotion.Status = "pending";
            var user = await _usermanger.FindByIdAsync(userId);
            if(user==null)
            return BadRequest();
            promotion.UserId = userId;
            _repo.Add(promotion);
            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> updatePromotion(string userId,PromoDto promo){
            var promotion = _mapper.Map<Promo>(promo);
            promotion.Status = "pending";
            promotion.Id = promo.Id;

            try
            {
                // save 
                await _promoService.UpdatePromo(promotion);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("user/active/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> getAllActivePromosOfSeller(string userId){

            var promotions = await _promoService.GetAllActivePromosOfSeller(userId);

            return Ok(promotions);
        }

        [HttpGet("user/pending/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> getAllPendingPromosOfSeller(string userId){

            var promotions = await _promoService.GetAllPendingPromosOfSeller(userId);

            return Ok(promotions);
        }

        

        
        
    }
}