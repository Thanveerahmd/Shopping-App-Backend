﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Project.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Project.Services;
using Project.Dtos;
using Project.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using pro.backend.Controllers;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Token _token;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _usermanger;
        private readonly SignInManager<User> _signmanger;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment hostingEnv;
        private const string ChampionsImageFolder = "images";
        public readonly iShoppingRepo _repo;

        public UsersController(
            Token token,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            UserManager<User> usermanger,
            SignInManager<User> signmanger,
            IEmailSender EmailSender,
            IHostingEnvironment hostingEnv,
            iShoppingRepo repo
            )
        {
            _usermanger = usermanger;
            _signmanger = signmanger;
            _emailSender = EmailSender;
            this.hostingEnv = hostingEnv;
            _repo = repo;
            _token = token;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody]UserDto userDto)
        {
            var user = await _usermanger.FindByNameAsync(userDto.Username);

            if (user != null)
            {
                var result = await _signmanger.CheckPasswordSignInAsync(user, userDto.Password, false);
                if (result.Succeeded)
                {

                    var appuser = await _usermanger.Users.FirstOrDefaultAsync(u =>
                       u.NormalizedUserName == userDto.Username.ToUpper());

                    var token = _token.GenrateJwtToken(appuser);
                    var cart = await _repo.GetCart(user.Id);
                    var cartToReturn = _mapper.Map<CartDto>(cart);
                    var image = await _repo.GetPhotoOfUser(user.Id);
                    string imageUrl = null;
                    if (image != null)
                        imageUrl = image.Url;
                    return Ok(new
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        imageurl = imageUrl,
                        Token = token,
                        cartToReturn
                    });
                }
                else
                {
                    var emailConfirmed = await _usermanger.IsEmailConfirmedAsync(user);
                    if (!await _usermanger.IsEmailConfirmedAsync(user))
                    {
                        return StatusCode(406);
                    }
                }
            }

            return Unauthorized();

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]UserDto userDto)
        {

            var createuser = _mapper.Map<User>(userDto);
            createuser.isLocked = false;
            var result = await _usermanger.CreateAsync(createuser, userDto.Password);
            var getuser = _mapper.Map<UserDto>(createuser);

            if (result != null && result.Succeeded)
            {
                var user = await _usermanger.FindByNameAsync(getuser.Username);

                var cart = new Cart();
                cart.BuyerId = getuser.Id;
                try
                {
                    _repo.Add(cart);

                    if (!(await _repo.SaveAll()))
                        return BadRequest(new { message = "Could not create Cart" });

                }
                catch (AppException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
                var useridentity = await _usermanger.FindByNameAsync(userDto.Username);
                // var userrole = await _usermanger.AddToRoleAsync(useridentity,userDto.Role);
                var code = await _usermanger.GenerateEmailConfirmationTokenAsync(createuser);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Users",
                new { userId = createuser.Id, code = code }, protocol: HttpContext.Request.Scheme);
                var mobileCode = System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.epizy.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                await _emailSender.SendEmailAsync(useridentity.Email, "Confirm your account",
             $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='{uri.AbsoluteUri}'>mobile link</a>");

                return Ok(new { Id = user.Id });
            }

            return BadRequest(result.Errors);

        }

        [AllowAnonymous]
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAsync([FromBody]UserDto userDto)
        {
            var useridentity = await _usermanger.FindByNameAsync(userDto.Username);

            if (useridentity != null)
            {

                // var userrole = await _usermanger.AddToRoleAsync(useridentity,userDto.Role);
                var code = await _usermanger.GenerateEmailConfirmationTokenAsync(useridentity);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Users",
                new { userId = useridentity.Id, code = code }, protocol: HttpContext.Request.Scheme);

                var mobileCode = System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.epizy.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                await _emailSender.SendEmailAsync(useridentity.Email, "Confirm your account",
                      $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='{uri.AbsoluteUri}'>mobile link</a>");

                return StatusCode(201);
            }
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {

            if (userId == null || code == null)
            {
                return StatusCode(400);
            }
            var user = await _usermanger.FindByIdAsync(userId);
            if (user == null)
            {
                return StatusCode(400);
            }
            var result = await _usermanger.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return StatusCode(200, "Email Confirmed");
            else return StatusCode(400);
        }

        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody]ForgetPasswordDto forgetPasswordDto)
        {

            var email = forgetPasswordDto.Email;
            var user = await _usermanger.FindByEmailAsync(email);

            if (user == null || !(await _usermanger.IsEmailConfirmedAsync(user)))
            {
                return StatusCode(404); // You dont have account 

            }
            else
            {
                var code = await _usermanger.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(nameof(ResetPassword), "Users",
                new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                var mobileCode = System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.epizy.com/passwordreset.php?id={user.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                await _emailSender.SendEmailAsync(email, "Reset Password for your Winkel account",
                      $"Please click this link to reset password:<a href='{uri.AbsoluteUri}'>mobile link</a>");

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
        public async Task<IActionResult> ResetPasswordConfirmation(ResetPasswordDto model)
        {

            var user = await _usermanger.FindByIdAsync(model.Id);

            var code = System.Net.WebUtility.UrlDecode(model.code);

            var result = await _usermanger.ResetPasswordAsync(user, code, model.Password);

            //code is the token produce when requesting forget password

            if (result.Succeeded)
            {
                return StatusCode(200, "Password reset successful!");
            }
            else
            {
                return StatusCode(400, "Error while resetting the password!");
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserDto model)
        {

            var user = await _usermanger.FindByIdAsync(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            if (model.Password != null)
                await _usermanger.ChangePasswordAsync(user, model.OldPassword, model.Password);

            user.Role = model.Role;

            // if (model.imageUrl != null)
            // {

            //     var file = Convert.FromBase64String(model.imageUrl);
            //     var filename = user.Id;
            //     var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", filename + ".jpg");
            //     using (var imageFile = new FileStream(path, FileMode.Create))
            //     {
            //         imageFile.Write(file, 0, file.Length);
            //         imageFile.Flush();
            //     }


            //     var pic = Path.Combine(hostingEnv.WebRootPath, ChampionsImageFolder);

            //     user.imageUrl = pic + "//" + filename + ".jpg";

            // }

            var result = await _usermanger.UpdateAsync(user);


            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return StatusCode(400, "Error while Update!");
            }
        }

        [AllowAnonymous]
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword(UpdateUserDto model)
        {

            var user = await _usermanger.FindByIdAsync(model.Id);

            var result = await _usermanger.ChangePasswordAsync(user, model.OldPassword, model.Password);


            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return StatusCode(400, "Error while Update!");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetailsById(string userId)
        {

            var user = await _usermanger.FindByIdAsync(userId);
            var image = await _repo.GetPhotoOfUser(user.Id);
            string imageUrl = null;
            if (image != null)
                imageUrl = image.Url;
            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                imageurl = imageUrl,
            });
        }

        [HttpGet("allBuyers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBuyers()
        {
            var users = await _usermanger.Users.FromSql("select * from AspNetUsers where Role='Both' OR Role ='Buyer'").Include(p => p.DeliveryDetails).Include(p => p.BillingInfo).ToListAsync();

            
            foreach (var item in users)
            {
              if(  await _usermanger.IsLockedOutAsync(item)){
                   item.isLocked =true;
              }else {
                   item.isLocked =false;
              }
              await _usermanger.UpdateAsync(item);
            }
            return Ok(users);
        }

        [HttpGet("allSellers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSellers()
        {
            var users = await _usermanger.Users.FromSql("select * from AspNetUsers where Role='Both' OR Role ='Seller'").Include(p => p.DeliveryDetails).Include(p => p.BillingInfo).ToListAsync();
            return Ok(users);
        }
    }
}
