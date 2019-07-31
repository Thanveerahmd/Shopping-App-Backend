using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Dtos;
using Project.Entities;
using Project.Helpers;
using Project.Services;


namespace Project.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminControllers : ControllerBase

    {
        private iAdminServices _adminService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly iAdvertisement _adService;
        private readonly UserManager<User> _usermanger;
        private IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly iPromoService _promoService;




        public AdminControllers(
            iAdminServices adminService,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            iPromoService promoService,
            IEmailSender EmailSender,
            iAdvertisement AdService,
            UserManager<User> usermanger
            )
        {
            _userService = userService;
            _adminService = adminService;
            _mapper = mapper;
            _emailSender = EmailSender;
            _promoService = promoService;
            _appSettings = appSettings.Value;
            _adService = AdService;
            _usermanger = usermanger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AdminDto userDto)
        {

            var user = _adminService.AuthenticateUser(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            if (user.IsEmailConfirmed == false)
            {
                user.ActivationCode = Guid.NewGuid().ToString();
                var code = user.ActivationCode;
                _adminService.UpdateAdmin(user);

                var userId = user.Username;
                var url = $"http://localhost:4200/emailVerification/{userId}?code={code}";
                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Admin",
                new { userId = user.Username, code = code }, protocol: HttpContext.Request.Scheme);
                // var mobileCode = System.Net.WebUtility.UrlEncode(code);
                // var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                //  Uri uri = new Uri(mobileCallbackUrl);
                _emailSender.SendEmailAsync(user.Username, "Confirm your account",
            $"Please confirm your account by clicking this link: <a href='{url}'>link</a>");
                return StatusCode(401);
            }
            else
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    // token dont expire 
                    Expires = DateTime.UtcNow.AddHours(12),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                // return basic user info (without password) and token to store client side
                return Ok(new
                {
                    Id = user.Id,
                    firstLogin = user.FirstLogin,
                    Username = user.Username, //follow the vedio if it failed change Task to normal process
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });
            }

        }


        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUser();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody]AdminDto userDto)
        {

            var user = _mapper.Map<Admin>(userDto);

            try
            {
                _adminService.AddAdmin(user, userDto.Password);

                var useridentity = _adminService.GetByEmail(userDto.Username);
                useridentity.ActivationCode = Guid.NewGuid().ToString();
                var code = useridentity.ActivationCode;
                _adminService.UpdateAdmin(useridentity);

                var userId = useridentity.Username;
                var url = $"http://localhost:4200/emailVerification/{userId}?code={code}";

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Admin",
                new { userId = useridentity.Username, code = code }, protocol: HttpContext.Request.Scheme);

                _emailSender.SendEmailAsync(useridentity.Username, "Confirm your account",
            $"Please confirm your account by clicking this link: <a href='{url}'>link</a>");

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(string userId, string code)
        {
            var user = _adminService.GetByEmail(userId);
            if (userId == null || code == null || user == null)
            {
                return StatusCode(400);
            }
            else
            {
                if (user.ActivationCode == code)
                {
                    user.IsEmailConfirmed = true;
                    _adminService.UpdateAdmin(user);
                    return Ok("Email Confirned");
                }
                else
                {
                    return Unauthorized("UnAuthorized");
                }
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var users = _adminService.GetAllAdmins();
            var userDtos = _mapper.Map<IList<AdminDto>>(users);
            return Ok(userDtos);
        }

        [HttpPut]
        public IActionResult Update([FromBody]AdminDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<Admin>(userDto);
            user.Id = userDto.Id;
            user.IsEmailConfirmed = true;



            try
            {
                // save 
                _adminService.UpdateAdmin(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{adminid}")]
        public IActionResult Delete(int id)
        {
            _adminService.DeleteAdmin(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {

            var id = forgetPasswordDto.Email;
            var user = _adminService.GetByEmail(id);

            if (user == null || !user.IsEmailConfirmed)
            {
                return StatusCode(404); // You dont have account 

            }
            else
            {
                user.ActivationCode = Guid.NewGuid().ToString();
                var code = user.ActivationCode;
                _adminService.UpdateAdmin(user);

                var url = $"http://localhost:4200/resetPassword/{forgetPasswordDto.Email}?code={code}";
                var callbackUrl = Url.Action(nameof(ResetPassword), "Admin",
                new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                // var mobileCode = System.Net.WebUtility.UrlEncode(code);
                //var mobileCallbackUrl = $"http://mahdhir.gungoos.com/passwordreset.php?id={user.Id}&code={mobileCode}";
                // Uri uri = new Uri(mobileCallbackUrl);
                _emailSender.SendEmailAsync(id, "Reset Password for your account",
                     $"Please click this link to reset password:<a href='{ url }'>link</a>");

                return StatusCode(201); // Password Resetting email is send
            }
        }

        [AllowAnonymous]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return Ok("Error");
            }
            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public IActionResult ResetPasswordConfirmation(AdminPasswordResetDto model)
        {

            var user = _adminService.GetByEmail(model.Username);

            if (user.ActivationCode == model.ActivationCode)
            {
                _adminService.UpdateAdminPassword(user, model.Password);
                return StatusCode(200, "Password reset successful!");
            }
            else
            {
                return StatusCode(400, "Error while resetting the password!");
            }
        }

        [HttpGet("pendingAdvertisements")]
        [AllowAnonymous]
        public IActionResult GetAllPendingAdvertisement()
        {
            var ad = _adService.GetPendingAdvertisement().Result;
            return Ok(ad);
        }

        [HttpGet("activeAdvertisements")]
        [AllowAnonymous]
        public IActionResult GetAllActiveAdvertisement()
        {
            var ad = _adService.GetActiveAdvertisement().Result;
            return Ok(ad);
        }

        [HttpGet("rejectedAdvertisements")]
        [AllowAnonymous]
        public IActionResult GetAllRejectedAdvertisement()
        {
            var ad = _adService.GetRejectedAdvertisement().Result;
            return Ok(ad);
        }

        [HttpGet("advertisements/{userId}")]
        [AllowAnonymous]
        public IActionResult GetAllAdvertisementOfaSeller(string userId)
        {
            var ad = _adService.GetAllAdvertisementOfSeller(userId).Result;
            return Ok(ad);
        }
        [AllowAnonymous]
        [HttpPut("approval/{reason}")]
        public IActionResult approval([FromBody]AdvertismentUploadDto adDto, string reason)
        {
            // status UserId is need
            var ad = _mapper.Map<Advertisement>(adDto);
            var prevAd = _adService.GetAdvertisement(adDto.Id).Result;
            prevAd.Status = adDto.Status;
            prevAd.DateAdded = DateTime.Now;
            var sellerId = prevAd.UserId;
            var seller = _usermanger.FindByIdAsync(sellerId).Result;

            try
            {
                _adService.UpdateAdvertisement(prevAd);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            if (ad.Status.ToLower().Equals("accepted"))
            {
                var url = $"http://mahdhir.epizy.com/advert.php?id={ad.Id}";

                _emailSender.SendEmailAsync(seller.UserName, "Payment For Advertisement",
               $"Please open this link from your mobile and proceed to payment: <a href='{url}'>link</a>");

                return Ok();

            }
            else if (ad.Status.ToLower().Equals("rejected"))
            {
                _emailSender.SendEmailAsync(seller.UserName, "Rejected Advertisement",
              $"Your Advertisement for product {ad.ProductId} has been rejected.\nReason:{reason}");

                return Ok();
            }
            else
            {
                return BadRequest("Status is Required");
            }

        }

        [AllowAnonymous]
        [HttpPost("LockUser/{userId}/{forDays?}")]
        public virtual async Task<IdentityResult> LockUserAccount(string userId,int? forDays)
        {
            var user = await _usermanger.FindByIdAsync(userId);
            var result = await _usermanger.SetLockoutEnabledAsync(user, true);
            if (result.Succeeded)
            {
                if (forDays.HasValue)
                {
                    result = await _usermanger.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(forDays.Value));
                    await _emailSender.SendEmailAsync(user.UserName, "About Your Account Activation",
               $"Please note that your Account has been blocked for {forDays} Days due violation of Our Rules and Regulations , contact us in Winkel@gmail.com");
                }
                else
                {
                    result = await _usermanger.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    await _emailSender.SendEmailAsync(user.UserName, "About Your Account Activation",
               $"Please note that your Account has been blocked Permently due violation of Our Rules and Regulations  , contact us in Winkel@gmail.com");
                }
                

            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost("UnLockUser/{userId}")]
        public virtual async Task<IdentityResult> UnlockUserAccount(string userId)
        {
            var user = await _usermanger.FindByIdAsync(userId);
            var result = await _usermanger.SetLockoutEnabledAsync(user, false);
            if (result.Succeeded)
            {
                await _usermanger.ResetAccessFailedCountAsync(user);
                 await _emailSender.SendEmailAsync(user.UserName, "About Your Account Activation",
               $"Please note that your Account has been Reactivated  ,contact us in Winkel@gmail.com");
            }
            return result;
        }

        [HttpPut("promo/approval/{reason}")]
        [AllowAnonymous]
        public async Task<IActionResult> updatePromoStatus(PromoStatusUpdateDto promo,string reason){
            var prevPromo = await _promoService.GetPromo(promo.Id);
            var sellerId= prevPromo.UserId;
            var seller = await _usermanger.FindByIdAsync(sellerId);
             try
            {
                // save 
                await _promoService.UpdatePromoStatus(promo.Id,promo.Status);
                  if (promo.Status.ToLower().Equals("accepted"))
            {
                

                await _emailSender.SendEmailAsync(seller.UserName, "Accepted Promotion",
               $"Your Advertisement for product {prevPromo.ProductId} has been accepted.");

                return Ok();

            }
            else if (promo.Status.ToLower().Equals("rejected"))
            {
                await _emailSender.SendEmailAsync(seller.UserName, "Rejected Advertisement",
              $"Your Advertisement for product {prevPromo.ProductId} has been rejected.\nReason:{reason}");

                return Ok();
            }
            else
            {
                return BadRequest("Status is Required");
            }

            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("promo/active")]
        [AllowAnonymous]
        public async Task<IActionResult> getAllActivePromos(){

            var promotions = await _promoService.GetAllActivePromos();

            return Ok(promotions);
        }

        [HttpGet("promo/pending")]
        [AllowAnonymous]
        public async Task<IActionResult> getAllPendingPromos(){

            var promotions = await _promoService.GetAllPendingPromos();

            return Ok(promotions);
        }
    }




}