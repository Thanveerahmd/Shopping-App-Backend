using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
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
        public ProductController(iProductService productService,
        IMapper mapper)
        {
            _mapper = mapper;
            _productService = productService;
        }
    [HttpGet("products")]
    public IActionResult GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        var productDtos = _mapper.Map<IList<ProductDto>>(products);
        return Ok(productDtos);
    }

    [HttpGet("products/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetById(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost("addProduct")]
        public IActionResult AddProduct([FromBody]ProductDto productDto)
        {
            // map dto to entity
            var product = _mapper.Map<Product>(productDto);
            try{
                _productService.AddProduct(product);
                return Ok();

            }catch (AppException ex){
                
                return BadRequest(new { message = ex.Message });
            }
            
        }
}
}