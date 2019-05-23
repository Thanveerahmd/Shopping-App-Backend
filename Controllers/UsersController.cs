using System;
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
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;
using pro.backend.Controllers;
using pro.backend.Dtos;

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

      
        public UsersController(
            Token token,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            UserManager<User> usermanger,
            SignInManager<User> signmanger,
            IEmailSender EmailSender,
            IHostingEnvironment hostingEnv
            )
        {
            _usermanger = usermanger;
            _signmanger = signmanger;
            _emailSender = EmailSender;
            this.hostingEnv = hostingEnv;
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
                       string image = null;
                        if (user.imageUrl != null) 
                        {
                        string path = user.imageUrl;
                        byte[] b = System.IO.File.ReadAllBytes(path);
                        image = "data:image/jpg;base64," + Convert.ToBase64String(b);
                        }
                        
                        var token = _token.GenrateJwtToken(appuser);
                    return Ok(new
                    {
                        Id = user.Id,
                        // Imageurl = user.imageUrl, // newly added
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        imageurl = image,
                        Token = token
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
            // map dto to entity
            var createuser = _mapper.Map<User>(userDto);
            var result = await _usermanger.CreateAsync(createuser, userDto.Password);
            var getuser = _mapper.Map<UserDto>(createuser);

            if (result != null && result.Succeeded)
            {
                var user = await _usermanger.FindByNameAsync(getuser.Username);
                if (userDto.imageUrl != null)
                {

                var file = Convert.FromBase64String(userDto.imageUrl);
                var filename = user.Id;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", filename + ".jpg");
                using (var imageFile = new FileStream(path, FileMode.Create))
                {
                    imageFile.Write(file, 0, file.Length);
                    imageFile.Flush();
                }
                
                
                var pic = Path.Combine(hostingEnv.WebRootPath, ChampionsImageFolder);
               
                user.imageUrl = pic + "//" + filename + ".jpg";
                var result2 = await _usermanger.UpdateAsync(user);


                }
                
                var useridentity = await _usermanger.FindByNameAsync(userDto.Username);
               // var userrole = await _usermanger.AddToRoleAsync(useridentity,userDto.Role);
                var code = await _usermanger.GenerateEmailConfirmationTokenAsync(createuser);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Users",
                new { userId = createuser.Id, code = code }, protocol: HttpContext.Request.Scheme);
                var mobileCode = System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                await _emailSender.SendEmailAsync(useridentity.Email, "Confirm your account",
             $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='{uri.AbsoluteUri}'>mobile link</a>");

                return StatusCode(201);
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
                var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
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
                var mobileCallbackUrl = $"http://mahdhir.gungoos.com/passwordreset.php?id={user.Id}&code={mobileCode}";
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
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateUserDto model)
        {
            
            var user = await _usermanger.FindByIdAsync(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PasswordHash = model.Password;
              var result = await _usermanger.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                return StatusCode(200, "Update successful!");
            }
            else
            {
                return StatusCode(400, "Error while Update!");
            }
        }

    }
}
