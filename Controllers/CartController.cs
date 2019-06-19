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

        [HttpPost("addtocart")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCartProduct([FromBody]CartProductDto productDto)
        {

            //map dto to entity
            var cart = _mapper.Map<Cart>(productDto);
            try
            {
                _repo.Add(cart);
                //return Ok(product.CartId);

            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }


             //var cart = await _repo.GetCart(product.CartId);
             var cartProduct = _mapper.Map<CartProduct>(productDto);
             cart.CartDetails.Add(cartProduct);
             //product.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
               // var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                //return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                return Ok(cart.Id);
            }
            return BadRequest("Coudn't add the Product to Cart");


        }
        // [HttpGet("{id}", Name = "GetPhoto")]
        // [AllowAnonymous]
        // public async Task<IActionResult> GetPhoto(int id)
        // {
        //     var photoFromRepo = await _repo.GetPhoto(id);

        //     var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

        //     return Ok(photo);
        // }

    }

}
