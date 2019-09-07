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
        private readonly iCategoryService _categoryService;
        private readonly iAnalytics _analyticsService;
        private static readonly HttpClient Client = new HttpClient();

        public ProductController(iProductService productService,
        IMapper mapper, iShoppingRepo repo, iCategoryService categoryService,
        iAnalytics analyticsService)
        {
            _mapper = mapper;
            _repo = repo;
            _categoryService = categoryService;
            _productService = productService;
            _analyticsService = analyticsService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repo.GetAllProducts();
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);

        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var identity = HttpContext.User.Identity;
            string userId = "";
            if (identity != null)
            {
                userId = identity.Name;
            }

            var product = await _repo.GetProduct(id);
            if (userId != "" && userId != null)
            {
                var previousSubCategoryView = await _analyticsService.GetPageViewRecordIfAvailable(product.Sub_category, userId);
                var oldvisit = await _analyticsService.GetProductViewRecordIfAvailable(product.Id, userId);
                
                if (oldvisit == null)
                {
                    ProductView data = new ProductView();
                    data.UserId = userId;
                    data.ProductId = product.Id;
                    data.NoOfVisits = 1;
                    data.LatestVisit = DateTime.Now;
                    _repo.Add(data);
                }
                else
                {
                    await _analyticsService.UpdateProductViewRecord(oldvisit);
                }

                if (previousSubCategoryView == null)
                {
                    PageViews data = new PageViews();
                    data.UserId = userId;
                    data.Sub_category = product.Sub_category;
                    data.Sub_categoryId = product.Sub_categoryId;
                    data.NoOfVisits = 1;
                    data.LatestVisit = DateTime.Now;
                    await _analyticsService.AddPageViewRecord(data);
                }
                else
                {
                    await _analyticsService.UpdatePageViewRecord(previousSubCategoryView);
                }
            }

            var productToReturn = _mapper.Map<ProductDto>(product);
            var RecommendedProducts = await _repo.GetProductsBySearchQuery(productToReturn.Sub_category, "Sub Category");
            var RecommendedProductsToReturn = _mapper.Map<ICollection<ProductListDto>>(RecommendedProducts);

            IList<ProductListDto> Products = new List<ProductListDto>();

            if (RecommendedProductsToReturn.Count > 5)
            {
                var Prod = await _repo.GetProductsByNameAndSubCategory(id);
                var ProductsToReturn = _mapper.Map<ICollection<ProductListDto>>(Prod);
                foreach (var item in ProductsToReturn)
                {
                    if (item.Id != id)
                        Products.Add(item);
                    if (Products.Count == 5)
                        break;


                }
                if (Products.Count < 5)
                {
                    foreach (var items in RecommendedProductsToReturn)
                    {
                        if (items.Id != id)
                            Products.Add(items);

                        if (Products.Count == 5)
                            break;
                    }
                }
            }
            else
            {
                foreach (var item in RecommendedProductsToReturn)
                {
                    if (item.Id != id)
                        Products.Add(item);

                    if (Products.Count == 5)
                        break;
                }
            }

            var RecommendedProduct = _analyticsService.getRecommendation(id);
            var ProductRecommended = _mapper.Map<ICollection<ProductListDto>>(RecommendedProduct);

            return Ok(new { product = productToReturn, similarProducts = Products, RecommendedProducts = ProductRecommended });


        }

        [HttpPost("addProduct/{SubCategoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddProduct([FromBody]ProductAddingDto productDto, int SubCategoryId)
        {
            // map dto to entity
            var product = _mapper.Map<Product>(productDto);
            var SubCategory = await _categoryService.GetSubCategorywithPhoto(SubCategoryId);


            if (SubCategory == null)
            {
                return BadRequest(new { message = "There is No such SubCategory" });

            }

            var category = await _categoryService.GettheCategory(SubCategory.CategoryId);

            if (category == null)
            {
                return BadRequest(new { message = "There is No such Category" });

            }

            product.Sub_category = SubCategory.SubCategoryName;
            product.Category = category.CategoryName;
            product.Sub_categoryId = SubCategoryId;
            product.CategoryId = category.Id;

            try
            {
                // string text = product.Product_name + " " + product.Product_Discription;

                // Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.Ocp_Apim_Subscription_Key);

                // var response = await Client.PostAsync(Keys.TextModerationUrl, new StringContent(text));
                // var contents = await response.Content.ReadAsStringAsync();
                // var jo = JObject.Parse(contents);
                // if (jo != null && jo["Classification"] != null && jo["Classification"]["ReviewRecommended"] != null)
                // {
                //     var flag_status = jo["Classification"]["ReviewRecommended"];
                //     if (flag_status.ToObject<bool>())
                //     {
                //         product.visibility = false;
                //     }
                //     else
                //     {
                //         product.visibility = true;
                //     }
                // }

                //TESTING PURPOSES
                product.visibility = true;

                SubCategory.Products.Add(product);
                category.SubCategorys.Add(SubCategory);
                _productService.AddProduct(product);
                await _categoryService.UpdateSubCategory(SubCategory);
                await _categoryService.UpdateCategory(category);

                return Ok(new { id = product.Id, visibility = product.visibility });

            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("{ProductId}")]
        public IActionResult Delete(int ProductId)
        {
            _productService.DeleteProduct(ProductId);
            return Ok();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProduct([FromBody]ProductDto productDto)
        {
            var prod = _mapper.Map<Product>(productDto);
            prod.Id = productDto.Id;

            // var oldproduct = await _repo.GetProduct(prod.Id);
            // var SubCategory = await _categoryService.GetSubCategorywithPhoto(oldproduct.Sub_categoryId);
            // var Category = await _categoryService.GettheCategory(oldproduct.CategoryId);

            //  var newSubCategory = await _categoryService.GetSubCategorywithPhoto(SubCategoryId);
            // SubCategory.Products.Remove(oldproduct);

            // if (newSubCategory == null)
            // {
            //     return BadRequest(new { message = "There is No such SubCategory" });

            // }

            // var newcategory = await _categoryService.GettheCategory(SubCategory.CategoryId);

            // if (newcategory == null)
            // {
            //     return BadRequest(new { message = "There is No such Category" });

            // }

            // prod.Sub_category = newSubCategory.SubCategoryName;
            // prod.Category = newcategory.CategoryName;
            // prod.Sub_categoryId = SubCategoryId;
            // prod.CategoryId = newcategory.Id;

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
                        else
                        {
                            prod.visibility = true;
                        }
                    }
                }
                await _productService.UpdateProduct(prod);
                //await _categoryService.UpdateSubCategory(newSubCategory);
                return Ok(new { visibility = prod.visibility });
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

        [HttpGet("seller/unflagged/{sellerID}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUnflagedProductsOfSeller(string sellerID)
        {
            var products = await _repo.GetAllUnflaggedProductsOfSeller(sellerID);
            var productsToReturn = _mapper.Map<IEnumerable<ProductListDto>>(products);
            return Ok(productsToReturn);
        }


        [HttpGet("{parameter}/{searchQuery}/{type?}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByQuery(string searchQuery, string parameter, string type)
        {
            var identity = HttpContext.User.Identity;
            string userId = "";
            if (identity != null)
            {
                userId = identity.Name;
            }

            if (type == "click" && userId != "" && userId != null)
            {
                var keyWordArray = searchQuery.Split(" ");
                foreach (var item in keyWordArray)
                {
                    var prevRecord = await _analyticsService.GetBuyerSearchRecordIfAvailable(item.ToLower(), userId);
                    if (prevRecord == null)
                    {
                        var Record = new BuyerSearch();
                        Record.Keyword = item.ToLower();
                        Record.UserId = userId;
                        Record.NoOfSearch = 1;
                        Record.LatestVisit = DateTime.Now;
                        await _analyticsService.AddBuyerSearchRecord(Record);
                    }
                    else
                    {
                        await _analyticsService.UpdateBuyerSearchRecord(prevRecord);
                    }
                }
            }
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
            {
                var product = await _repo.GetProduct(rate.ProductId);
                var sum = 0;
                var count = 0;
                foreach (var item in product.Ratings)
                {
                    sum += item.RatingValue;
                    count++;
                }
                var avg = (float)sum / count;

                product.rating = avg;

                if (await _productService.UpdateProduct(product))
                    return Ok(rate.Id);

                return BadRequest();

            }
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

                var product = await _repo.GetProduct(rate.ProductId);
                var sum = 0;
                var count = 0;
                foreach (var item in product.Ratings)
                {
                    sum += item.RatingValue;
                    count++;
                }
                var avg = (float)sum / count;

                product.rating = avg;

                await _productService.UpdateProduct(product);
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


        [HttpPost("pricesuggestion/{SubCategoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPriceSuggestion([FromBody]ProductAddingDto productDto, int SubCategoryId)
        {

            var product = _mapper.Map<Product>(productDto);

            var SubCategory = await _categoryService.GetSubCategorywithPhoto(SubCategoryId);

            if (SubCategory == null)
            {
                return BadRequest(new { message = "There is No such SubCategory" });

            }

            try
            {
                string text = product.Product_name + " " + product.Product_Discription;
                var SuggestPrice = await _analyticsService.getPriceSuggestions(SubCategoryId, product.Product_Discription, product.Product_name);
                return Ok(SuggestPrice);
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
