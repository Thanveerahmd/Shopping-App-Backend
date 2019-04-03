using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

                public AdminControllers(
                    iAdminServices adminService,
                    IUserService userService,
                    IMapper mapper,
                    IOptions<AppSettings> appSettings)
                {
                     _userService = userService;
                    _adminService = adminService;
                    _mapper = mapper;
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
            return Ok(new {
                Id = user.Id,
                Username = user.Username, //follow the vedio if it failed change Task to normal process
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        } 

        
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users =  _userService.GetAllUser();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        
        [HttpPost("register")]
        public  IActionResult Register([FromBody]AdminDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<Admin>(userDto);

            try 
            {
                // save 
                _adminService.AddAdmin(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

         [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _adminService.GetAllAdmins();
            var userDtos = _mapper.Map<IList<AdminDto>>(users);
            return Ok(userDtos);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update([FromBody]AdminDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<Admin>(userDto);
            user.Id = userDto.Id;

            try 
            {
                // save 
                _adminService.UpdateAdmin(user, userDto.Password);
                return StatusCode(200);
            } 
            catch(AppException ex)
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
    }

    

}