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

        [HttpGet("products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repo.GetAllProducts();
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }

        [HttpGet("products/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repo.GetProduct(id);
            var productToReturn = _mapper.Map<ProductDto>(product);
            return Ok(productToReturn);
        }

        [HttpPost("addProduct")]
        [AllowAnonymous]
        public IActionResult AddProduct([FromBody]ProductDto productDto)
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
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);
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
    }
}