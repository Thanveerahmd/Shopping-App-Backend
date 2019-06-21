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
    [Route("delivery")]

    public class DeliveryController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;

        public DeliveryController(IMapper mapper, iShoppingRepo repo)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddDeliveryInfo(DeliveryInfoDto DeliveryInfoDto)
        {

            var deliveryInfo = _mapper.Map<DeliveryInfo>(DeliveryInfoDto);
            var info = await _repo.GetDeliveryInfosOfUser(deliveryInfo.UserId);
            deliveryInfo.isDefault = false;
            if (info == null)
                deliveryInfo.isDefault = true;

            _repo.Add(deliveryInfo);

            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Coudn't add the DeliveryInfo");
        }

        [HttpPut]
        [AllowAnonymous]
        public IActionResult UpdateDeliveryInfo(DeliveryInfoDto DeliveryUpdateInfoDto)
        {
            var prod = _mapper.Map<DeliveryInfo>(DeliveryUpdateInfoDto);
            prod.Id = DeliveryUpdateInfoDto.Id;

            try
            {
                // save 
                _repo.UpdateDeliveryInfo(prod);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDeliveryInfo(string UserId)
        {
            var info = await _repo.GetDeliveryInfosOfUser(UserId);
            var DeliveryInfo = _mapper.Map<IEnumerable<DeliveryInfoDto>>(info);
            return Ok(DeliveryInfo);
        }

        [HttpDelete("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Deleteinfo(int Id)
        {

            var info = await _repo.GetDeliveryInfo(Id);

            _repo.Delete(info);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to delete the Data");

        }

        [HttpPost("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> SetDefault(int Id)
        {
            var info = await _repo.GetDeliveryInfo(Id);
            var userId = info.UserId;
            info.isDefault = true;
            var prevDefaultRecord = await _repo.GetDeliveryInfoOfDefault(userId);
            prevDefaultRecord.isDefault = false;

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to change default");
        
        }
    }
}