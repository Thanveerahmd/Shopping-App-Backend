using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.Helpers;
using pro.backend.iServices;
using Project.Helpers;
using System.Text;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("delivery")]

    public class DeliveryController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        private static readonly HttpClient Client = new HttpClient();

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
            if (info.Count == 0)
                deliveryInfo.isDefault = true;

            _repo.Add(deliveryInfo);

            if (await _repo.SaveAll())
            {
                return Ok(deliveryInfo.Id);
            }
            return BadRequest();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDeliveryInfo(DeliveryInfoDto DeliveryUpdateInfoDto)
        {
            var prod = _mapper.Map<DeliveryInfo>(DeliveryUpdateInfoDto);
            prod.Id = DeliveryUpdateInfoDto.Id;

            try
            {
                // save 
                await _repo.UpdateDeliveryInfo(prod);
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
            var list = await _repo.GetDeliveryInfosOfUser(info.UserId);
            if (list.Count != 1)
            {
                var data = await _repo.SetAlternateDefault(info.UserId);
                data.isDefault = true;
            }
            _repo.Delete(info);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();

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
            return BadRequest();

        }

        [HttpPost("billing")]
        [AllowAnonymous]
        public async Task<IActionResult> AddBillingInfo(DeliveryInfoDto DeliveryInfoDto)
        {
            var BillingInfo = _mapper.Map<BillingInfo>(DeliveryInfoDto);
            var info = await _repo.GetBillingInfosOfUser(BillingInfo.UserId);
            BillingInfo.isDefault = false;
            if (info.Count == 0)
                BillingInfo.isDefault = true;

            string code = OTPGenerate.OTPCharacters();
            string massege_body = "Your OTP is " + code + "%0a http://bit.do/eYdZE?otp=" + code;
            BillingInfo.OTP = code;
            if (BillingInfo.OTP != null)
            {
                BillingInfo.isOTP = true;
            }
            else BillingInfo.isOTP = false;

            // HttpContent content = null;

            // await Client.PostAsync($"http://sms.techwirelanka.com/SMSAPIService.svc/SmsApi/TECHWIRE/{DeliveryInfoDto.MobileNumber}/{massege_body}/winkel/password", content);
            BillingInfo.OTPCount++;

            _repo.Add(BillingInfo);

            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("billing")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateBillingInfo(DeliveryInfoDto BillingUpdate)
        {
            var info = _mapper.Map<BillingInfo>(BillingUpdate);
            info.Id = BillingUpdate.Id;

            var prev = await _repo.GetBillingInfo(BillingUpdate.Id);
            info.OTP = prev.OTP;
            info.isDefault = prev.isDefault;
            info.isMobileVerfied = prev.isMobileVerfied;
            info.isOTP = prev.isOTP;
            bool val = prev.MobileNumber != BillingUpdate.MobileNumber;
            if (prev.MobileNumber != BillingUpdate.MobileNumber)
            {
                string code = OTPGenerate.OTPCharacters();
                string massege_body = "Your OTP is " + code + "%0a http://bit.do/eYdZE?otp=" + code;
                info.OTP = code;
                if (info.OTP != null)
                {
                    info.isOTP = true;
                }
                else info.isOTP = false;
                info.isMobileVerfied = false;
                // HttpContent content = null;

                // await Client.PostAsync($"http://sms.techwirelanka.com/SMSAPIService.svc/SmsApi/TECHWIRE/{DeliveryInfoDto.MobileNumber}/{massege_body}/winkel/password", content);
                info.OTPCount++;
            }

            try
            {
                await _repo.UpdateBillingInfo(info);
                return Ok(new { mobileChanged = val });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("authenticate/{user_Id}/{OTP}")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticatePhoneNumber(string user_Id, string OTP)
        {
            var info = await _repo.GetBillingInfobyOtp(user_Id, OTP);

            if (info == null)
            {
                return BadRequest();

            }
            else
            {

                info.OTP = null;
                info.isOTP = false;
                info.isMobileVerfied = true;
                info.OTPCount = 0;
                await _repo.SaveAll();

                return Ok();
            }
        }

        [HttpGet("billing/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBillingInfo(string UserId)
        {
            var info = await _repo.GetBillingInfosOfUser(UserId);
            var BillingInfo = _mapper.Map<IEnumerable<BillingInfoDto>>(info);

            return Ok(BillingInfo);
        }

        [HttpDelete("billing/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteBillingInfo(int Id)
        {

            var info = await _repo.GetBillingInfo(Id);
            var list = await _repo.GetBillingInfosOfUser(info.UserId);
            if (list.Count != 1)
            {
                var data = await _repo.AlternateDefault(info.UserId);
                data.isDefault = true;
            }
            _repo.Delete(info);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();

        }

        [HttpPost("billing/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> SetTheDefault(int Id)
        {
            var info = await _repo.GetBillingInfo(Id);
            var userId = info.UserId;
            info.isDefault = true;
            var prevDefaultRecord = await _repo.GetBillingInfoOfDefault(userId);
            prevDefaultRecord.isDefault = false;

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();

        }

        [HttpGet("billing/default/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDefaultBillingInfo(string UserId)
        {
            var info = await _repo.GetBillingInfoOfDefault(UserId);
            var BillingInfo = _mapper.Map<BillingInfoDto>(info);

            return Ok(BillingInfo);
        }



        [HttpGet("default/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDefaultDeliveryInfo(string UserId)
        {
            var info = await _repo.GetDeliveryInfoOfDefault(UserId);
            var BillingInfo = _mapper.Map<DeliveryInfoDto>(info);

            return Ok(BillingInfo);
        }

        [HttpPost("otpresend/{UserId}/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOTP(string UserId, int Id)
        {

            var BillingInfo = await _repo.GetBillingInfo(Id);
            if (BillingInfo == null)
            {
                return BadRequest(new { message = "Invalid Data" });
            }
            if (BillingInfo.isMobileVerfied)
            {
                return BadRequest(new { message = "Mobile Number Already Verified" });
            }
            if (BillingInfo.OTPCount >= 2)
            {
                return BadRequest(new { message = "Sorry You Have Reached The OTP Limit" });
            }
            string code = OTPGenerate.OTPCharacters();
            string massege_body = "Your OTP is " + code + "%0a http://bit.do/eYdZE?otp=" + code;
            BillingInfo.OTP = code;
            if (BillingInfo.OTP != null)
            {
                BillingInfo.isOTP = true;
            }
            else BillingInfo.isOTP = false;

            // HttpContent content = null;

            // await Client.PostAsync($"http://sms.techwirelanka.com/SMSAPIService.svc/SmsApi/TECHWIRE/{BillingInfo.MobileNumber}/{massege_body}/winkel/password", content);

            BillingInfo.OTPCount++;
            await _repo.SaveAll();
            return Ok();
        }
    }
}