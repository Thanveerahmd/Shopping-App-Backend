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

namespace WebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _usermanger;
        private readonly SignInManager<User> _signmanger;
        private readonly IEmailSender _emailSender;

        public UsersController(
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            UserManager<User> usermanger,
            SignInManager<User> signmanger,
            IEmailSender EmailSender
            )
        {
            _usermanger = usermanger;
            _signmanger = signmanger;
            _emailSender = EmailSender;
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

                    return Ok(new
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role, 
                        Token = GenrateJwtToken(appuser)
                    });
                }else{
                    var emailConfirmed = await _usermanger.IsEmailConfirmedAsync(user);
                    if(!await _usermanger.IsEmailConfirmedAsync(user)){
                        return StatusCode(406);
                    }
                }
            }

            return Unauthorized();

        }

        private String GenrateJwtToken(User user)
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
                // Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]UserDto userDto)
        {
            // map dto to entity
            var createuser = _mapper.Map<User>(userDto);
            var result = await _usermanger.CreateAsync(createuser, userDto.Password);
            var getuser = _mapper.Map<UserDto>(createuser);


            if (result.Succeeded)
            {
                var useridentity = await _usermanger.FindByNameAsync(userDto.Username);
                // var userrole = await _usermanger.AddToRoleAsync(useridentity,userDto.Role);
                var code = await _usermanger.GenerateEmailConfirmationTokenAsync(createuser);

                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Users",
                new { userId = createuser.Id, code = code }, protocol: HttpContext.Request.Scheme);
                var mobileCode =  System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                         await _emailSender.SendEmailAsync(useridentity.Email, "Confirm your account",
                      $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='{uri.AbsoluteUri}'>mobile link</a>");

                //return CreatedAtRoute("GetUser", new{Controller="Users", id=createuser.Id },getuser);
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

                var mobileCode =  System.Net.WebUtility.UrlEncode(code);
                var mobileCallbackUrl = $"http://mahdhir.gungoos.com/winkel.php?id={useridentity.Id}&code={mobileCode}";
                Uri uri = new Uri(mobileCallbackUrl);
                await _emailSender.SendEmailAsync(useridentity.Email, "Confirm your account",
                      $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> or <a href='{uri.AbsoluteUri}'>mobile link</a>");

                //return CreatedAtRoute("GetUser", new{Controller="Users", id=createuser.Id },getuser);
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
                if(result.Succeeded)
                return StatusCode(200,"Email Confirmed");
                else return StatusCode(400);
        }



        // [HttpGet("Getusers")]
        // public IActionResult GetAll()
        // {
        //     var users =  _userService.GetAllUser();
        //     var userDtos = _mapper.Map<IList<UserDto>>(users);
        //     return Ok(userDtos);
        // }

        // [HttpGet("{id}")]
        // public IActionResult GetById(int id)
        // {
        //    // var user =  _userService.GetById(id);
        //     var userDto = _mapper.Map<UserDto>(user);
        //     return Ok(userDto);
        // }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            //  user.Id = id;

            try
            {
                // save 
                //     _userService.UpdateUser(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //  _userService.DeleteUser(id);
            return Ok();
        }
    }
}
