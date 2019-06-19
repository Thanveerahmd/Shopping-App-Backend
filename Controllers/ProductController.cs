using System;
using System.Collections.Generic;
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
    [Route("product")]

    public class ProductController : ControllerBase
    {
        private readonly iProductService _productService;
        private readonly IMapper _mapper;

        public readonly iShoppingRepo _repo;

        public ProductController(iProductService productService,
        IMapper mapper, iShoppingRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repo.GetAllProducts();
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repo.GetProduct(id);
            var productToReturn = _mapper.Map<ProductDto>(product);
            return Ok(productToReturn);
        }

        [HttpPost("addProduct")]
        [AllowAnonymous]
        public IActionResult AddProduct([FromBody]ProductAddingDto productDto)
        {
            // map dto to entity
            var product = _mapper.Map<Product>(productDto);
            try
            {
                _productService.AddProduct(product);
                return Ok(product.Id);

            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("{productid}")]
        public IActionResult Delete(int productid)
        {
            _productService.DeleteProduct(productid);
            return Ok();
        }

        [HttpPut]
        [AllowAnonymous]
        public IActionResult UpdateProduct([FromBody]ProductDto productDto)
        {
            // map dto to entity and set id
            var prod = _mapper.Map<Product>(productDto);
            prod.Id = productDto.Id;

            try
            {
                // save 
                _productService.UpdateProduct(prod);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("seller/{sellerID}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProduct(string sellerID)
        {
            var products = await _repo.GetAllProductsOfSeller(sellerID);
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }

        [HttpGet("{parameter}/{searchQuery}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByQuery(string searchQuery, string parameter){

            var products = await _repo.GetProductsBySearchQuery(searchQuery,parameter);
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }
    }
}
