using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using pro.backend.Dtos;
using pro.backend.Entities;
using Project.Entities;
using Project.Helpers;
using Microsoft.AspNetCore.Authorization;
using pro.backend.iServices;
using AutoMapper;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("socialauth")]
    public class SocialAuthController : Controller
    {
        private readonly DataContext _appDbContext;
        private readonly Token _token;
        private readonly iShoppingRepo _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly FacebookAuthSettings _fbAuthSettings;
        private static readonly HttpClient Client = new HttpClient();

        public SocialAuthController(IOptions<FacebookAuthSettings> fbAuthSettingsAccessor,
        UserManager<User> userManager,
        DataContext appDbContext,
        Token token,
        iShoppingRepo repo,
        IMapper mapper
          )
        {
            _fbAuthSettings = fbAuthSettingsAccessor.Value;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _token = token;
            _repo = repo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<IActionResult> Facebook([FromBody]SocialAuthDto model)
        {
            // 1.generate an app access token
            var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={Keys.FacebookAppId}&client_secret={Keys.FacebookAppSecret}&grant_type=client_credentials");
            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
            // 2. validate the user access token
            var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                return BadRequest("login_failure-Invalid facebook token");
            }

            // 3. we've got a valid token so we can request user data from fb
            var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            // 4. ready to create the local user account (if necessary) and jwt
            var user = await _userManager.FindByEmailAsync(userInfo.Email);


            if (user == null)
            {
                var appUser = new User
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    FacebookId = userInfo.Id,
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    Role = "Buyer",
                    imageUrl = userInfo.Picture.Data.Url
                };

                var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

                var cart = new Cart();
                cart.BuyerId = appUser.Id;
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

                if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

                // await _appDbContext.Users.AddAsync(new Customer { IdentityId = appUser.Id, Location = "", Locale = userInfo.Locale, Gender = userInfo.Gender });
                // await _appDbContext.SaveChangesAsync();
            }

            // generate the jwt for the local user...
            var localUser = await _userManager.FindByNameAsync(userInfo.Email);

            if (localUser == null)
            {
                return BadRequest("login_failure - Failed to create local user account");
            }

            var jwt = _token.GenrateJwtToken(localUser);


            var carts = await _repo.GetCart(localUser.Id);
            var cartToReturn = _mapper.Map<CartDto>(carts);

            return Ok(new
            {
                Id = localUser.Id,
                imageurl = localUser.imageUrl, // newly added
                Username = localUser.UserName,
                FirstName = localUser.FirstName,
                LastName = localUser.LastName,
                Role = localUser.Role,
                Token = jwt,
                cartToReturn
            });
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]SocialAuthDto model)
        {

            // 1. validate the user access token
            var userAccessTokenValidationResponse = "";
            try
            {
                userAccessTokenValidationResponse = await Client.GetStringAsync($"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={model.AccessToken}");
            }
            catch
            {
                return BadRequest("login_failure-Invalid google token");
            }
            var userAccessTokenValidation = JsonConvert.DeserializeObject<GoogleUserAccessTokenValidation>(userAccessTokenValidationResponse);

            // 2. we've got a valid token so we can request user data from fb
            var userInfoResponse = await Client.GetStringAsync($"https://www.googleapis.com/plus/v1/people/me?access_token={model.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<GoogleUserData>(userInfoResponse);

            // 3. ready to create the local user account (if necessary) and jwt
            var user = await _userManager.FindByEmailAsync(userInfo.Emails[0].Value);

            if (user == null)
            {
                var appUser = new User
                {
                    FirstName = userInfo.Name.GivenName,
                    LastName = userInfo.Name.FamilyName,
                    GoogleId = userInfo.Id,
                    Email = userInfo.Emails[0].Value,
                    UserName = userInfo.Emails[0].Value,
                    Role = "Buyer",
                    imageUrl = userInfo.Picture.Url
                };

                var cart = new Cart();
                cart.BuyerId = appUser.Id;
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

                var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

                if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

                // await _appDbContext.Users.AddAsync(new Customer { IdentityId = appUser.Id, Location = "", Locale = userInfo.Locale, Gender = userInfo.Gender });
                // await _appDbContext.SaveChangesAsync();
            }

            // generate the jwt for the local user...
            var localUser = await _userManager.FindByNameAsync(userInfo.Emails[0].Value);

            if (localUser == null)
            {
                return BadRequest("login_failure - Failed to create local user account");
            }

            var jwt = _token.GenrateJwtToken(localUser);

             var carts = await _repo.GetCart(localUser.Id);
            var cartToReturn = _mapper.Map<CartDto>(carts);


            return Ok(new
            {
                Id = localUser.Id,
                imageurl = localUser.imageUrl, // newly added
                Username = localUser.UserName,
                FirstName = localUser.FirstName,
                LastName = localUser.LastName,
                Role = localUser.Role,
                Token = jwt,
                cartToReturn
            });
        }



    }



}