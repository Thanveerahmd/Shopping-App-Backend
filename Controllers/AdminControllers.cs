using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using pro.backend.Dtos;
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
        private IUserService _userService;
        private readonly IEmailSender _emailSender;

        public AdminControllers(
            iAdminServices adminService,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
              IEmailSender EmailSender
            )
        {
            _userService = userService;
            _adminService = adminService;
            _mapper = mapper;
            _emailSender = EmailSender;
            _appSettings = appSettings.Value;

        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AdminDto userDto)
        {
            var user = _adminService.AuthenticateUser(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

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
            // map dto to entity
            var user = _mapper.Map<Admin>(userDto);

            try
            {
                // save 
                _adminService.AddAdmin(user, userDto.Password);

                var useridentity = _adminService.GetByEmail(userDto.Username);
                useridentity.ActivationCode = new System.Guid();
                var code = useridentity.ActivationCode;
                _adminService.UpdateAdmin(useridentity);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Admin",
                new { userId = useridentity.Username, code = code }, protocol: HttpContext.Request.Scheme);
                // var mobileCode = System.Net.WebUtility.UrlEncode(code);
                // var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                //  Uri uri = new Uri(mobileCallbackUrl);
                _emailSender.SendEmailAsync(useridentity.Username, "Confirm your account",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='uri.AbsoluteUri'>mobile link</a>");

                //return CreatedAtRoute("GetUser", new{Controller="Users", id=createuser.Id },getuser);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(string userId, System.Guid code)
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
                    return Unauthorized();
                }
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _adminService.GetAllAdmins();
            var userDtos = _mapper.Map<IList<AdminDto>>(users);
            return Ok(userDtos);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]AdminDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<Admin>(userDto);
            user.Id = id;

            try
            {
                // save 
                _adminService.UpdateAdmin(user);
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
        public IActionResult ForgetPassword([FromBody]AdminPasswordResetDto forgetPasswordDto)
        {

            var id = forgetPasswordDto.Username;
            var user = _adminService.GetByEmail(id);

            if (user == null || !user.IsEmailConfirmed)
            {
                return StatusCode(404); // You dont have account 

            }
            else
            {
                user.ActivationCode = Guid.NewGuid();
                var code = user.ActivationCode;
                _adminService.UpdateAdmin(user);

                var callbackUrl = Url.Action(nameof(ResetPassword), "Admin",
                new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                // var mobileCode = System.Net.WebUtility.UrlEncode(code);
                //var mobileCallbackUrl = $"http://mahdhir.gungoos.com/passwordreset.php?id={user.Id}&code={mobileCode}";
                // Uri uri = new Uri(mobileCallbackUrl);
                _emailSender.SendEmailAsync(id, "Reset Password for your Winkel account",
                     $"Please click this link to reset password:<a href='{ callbackUrl }'>mobile link</a>");

                return StatusCode(201); // Password Resetting email is send
            }
        }

        [AllowAnonymous]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string userId, System.Guid code)
        {
            if (userId == null || code == null)
            {
                return Ok("Error");
            }
            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public IActionResult ResetPasswordConfirmation([FromBody]AdminPasswordResetDto model)
        {

            var user = _adminService.GetByEmail(model.Username);

            if (user.ActivationCode == model.ActivationCode || model.ConfirmPassword == model.Password)
            {
                _adminService.UpdateAdminPassword(user, model.Password);
                return StatusCode(200, "Password reset successful!");
            }
            else
            {
                return StatusCode(400, "Error while resetting the password!");
            }
        }


    }




}