using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;
using Project.Helpers;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("cart")]
    public class CartController : ControllerBase
    {
        private readonly iProductService _productService;
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;

        public CartController(iProductService productService,
        IMapper mapper, iShoppingRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
            _productService = productService;
        }

        [HttpPost("addtocart/{buyerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCartProduct(CartProductDto productDto, string buyerId)
        {
            var cart = await _repo.GetCart(buyerId);
            var cartProduct = _mapper.Map<CartProduct>(productDto);
            cart.CartDetails.Add(cartProduct);
            if (await _repo.SaveAll())
            {
                // var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                //return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                return Ok(cart.Id);
            }
            return BadRequest();


        }


        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _repo.GetCart(userId);
            var cartToReturn = _mapper.Map<CartDto>(cart);
            return Ok(cartToReturn);
        }


        [HttpDelete("{User_Id}/{CartProductId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCartProduct(int CartProductId, string User_Id)
        {
            var cart = await _repo.GetCart(User_Id);

            if (!cart.CartDetails.Any(p => p.Id == CartProductId))
                return Unauthorized();
            
            var CartProduct = await _repo.GetCartProduct(CartProductId);

            _repo.Delete(CartProduct);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();
        }


       [HttpDelete("{CartId}")]
       [AllowAnonymous]
        public async Task<IActionResult> DeleteCart(int CartId)
        {
            var cartProducts = await _repo.GetAllCartProduct(CartId);

            _repo.DeleteAll(cartProducts);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();
        }
    }

}
