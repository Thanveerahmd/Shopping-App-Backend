using System;
using System.Net.Http;
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
using Project.Entities;
using Newtonsoft.Json.Linq;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("product")]

    public class ProductController : ControllerBase
    {
        private readonly iProductService _productService;
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;

        private static readonly HttpClient Client = new HttpClient();

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
        public async Task<IActionResult> AddProduct([FromBody]ProductAddingDto productDto)
        {
            // map dto to entity
            var product = _mapper.Map<Product>(productDto);

            try
            {
                string text = product.Product_name + " " + product.Product_Discription;

                Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);

                var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(text));
                var contents = await response.Content.ReadAsStringAsync();
                var jo = JObject.Parse(contents);
                if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
                {
                    var flag_status = jo["Classification"]["ReviewRecommended"];
                    if (flag_status.ToObject<bool>())
                    {
                        product.visibility = false;
                    }else{
                        product.visibility = true;
                    }
                }
                _productService.AddProduct(product);
                return Ok(product.Id);
                 //return Ok(product.Id,new {visibility = product.visibility});

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
        public async Task<IActionResult> UpdateProduct([FromBody]ProductDto productDto)
        {
            var prod = _mapper.Map<Product>(productDto);
            prod.Id = productDto.Id;

            try
            {
                if (prod.Product_name != null && prod.Product_Discription != null)
                {
                    string text = prod.Product_name + " " + prod.Product_Discription;

                    Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);

                    var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(text));
                    var contents = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(contents);
                    if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
                    {
                        var flag_status = jo["Classification"]["ReviewRecommended"];
                        if (flag_status.ToObject<bool>())
                        {
                            prod.visibility = false;
                        }
                    }
                }
                await _productService.UpdateProduct(prod);
                return Ok();
            }
            catch (AppException ex)
            {

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
        public async Task<IActionResult> GetProductsByQuery(string searchQuery, string parameter)
        {

            var products = await _repo.GetProductsBySearchQuery(searchQuery, parameter);
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }

        //NEED TO CHANGE
        [HttpPost("rating")]
        [AllowAnonymous]
        public async Task<IActionResult> AddProductRating(RatingDto rating)
        {

            var rate = _mapper.Map<Rating>(rating);

            if (rate.Comment != null)
            {
                Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);
                var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(rate.Comment));
                var contents = await response.Content.ReadAsStringAsync();
                var jo = JObject.Parse(contents);
                if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
                {
                    var flag_status = jo["Classification"]["ReviewRecommended"];
                    if (flag_status.ToObject<bool>())
                        return BadRequest(new { message = "Your Comment Has Been Found To Be Offensive" });
                }

            }

            _repo.Add(rate);

            if (await _repo.SaveAll())
                return Ok(rate.Id);
            return BadRequest();
        }

        [HttpPut("rating")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProductRating(RatingDto rating)
        {

            var rate = _mapper.Map<Rating>(rating);

            if (rate.Comment != null)
            {
                Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);
                var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(rate.Comment));
                var contents = await response.Content.ReadAsStringAsync();
                var jo = JObject.Parse(contents);
                if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
                {
                    var flag_status = jo["Classification"]["ReviewRecommended"];
                    if (flag_status.ToObject<bool>())
                        return BadRequest(new { message = "Your Comment Has Been Found To Be Offensive" });
                }

            }

            try
            {
                var prevRate = await _repo.GetRatingById(rate);
                if (rating.UserId != prevRate.UserId)
                {
                    return Unauthorized();
                }
                await _repo.UpdateRating(rate);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        [HttpDelete("rating")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteProductRating(RatingDto rating)
        {

            var rate = _mapper.Map<Rating>(rating);

            _repo.Delete(rate);
            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost("test/{text}")]
        [AllowAnonymous]
        public async Task<IActionResult> TestTextModeration(string text)
        {

            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);
            // Client.DefaultRequestHeaders.Add("Content-Type","text/plain");
            var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(text));
            var contents = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(contents);
            if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
            {
                var flag_status = jo["Classification"]["ReviewRecommended"];
                if (!flag_status.ToObject<bool>())
                {
                    Console.WriteLine(flag_status.ToObject<bool>());
                }
            }

            return Ok();

        }
    }
}
