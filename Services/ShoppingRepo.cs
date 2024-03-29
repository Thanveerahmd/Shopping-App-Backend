using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class ShoppingRepo : iShoppingRepo
    {
        private readonly DataContext _context;

        public ShoppingRepo(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }


        public void DeleteAll<T>(ICollection<T> entity) where T : class
        {
            _context.RemoveRange(entity);

        }

        public void AddAll<T>(ICollection<T> entity) where T : class
        {
            _context.AddRange(entity);

        }

        public async Task<IEnumerable<Product>> GetAllProducts()

        {
            var products = await _context.Products.Where(p => p.visibility != false).Include(p => p.Photos).ToListAsync();

            return products;
        }
        public async Task<IEnumerable<Product>> GetAllProductsOfSeller(string sellerID) //have to change
        {
            var products = await _context.Products.Where(p => p.SellerId == sellerID)
            .Include(p => p.Photos).ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> GetAllUnflaggedProductsOfSeller(string sellerID) //have to change
        {
            var products = await _context.Products.Where(p => p.SellerId == sellerID)
            .Where(p => p.visibility == true)
            .Include(p => p.Photos).ToListAsync();

            return products;
        }

        public async Task<Cart> GetCart(string id)
        {
            var cart = await _context.Cart.Include(p => p.CartDetails)
            .FirstOrDefaultAsync(i => i.BuyerId == id);

            return cart;
        }

        public async Task<Photo> GetMainPhotoForUserAsync(int ProductId)
        {
            return await _context.Photos.Where(u => u.ProductId == ProductId)
               .FirstOrDefaultAsync(p => p.isMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<IEnumerable<Product>> GetProductsBySearchQuery(string searchQuery, string parameter)
        {
            List<Product> products = null;
            searchQuery = searchQuery.ToLower();
            if (parameter == "Sub Category")
            {
                products = await _context.Products.Where(p => p.Sub_category.ToLower().Contains(searchQuery))
                .Where(p => p.visibility != false)
                .Include(p => p.Photos).ToListAsync();
            }
            else
            {
                if (parameter == "Category")
                {
                    products = await _context.Products.Where(p => p.Category.ToLower().Contains(searchQuery))
                    .Where(p => p.visibility != false)
                .Include(p => p.Photos).ToListAsync();
                }
                else
                {
                    if (parameter == "Name")
                    {
                        products = await _context.Products.Where(p => p.Product_name.ToLower().Contains(searchQuery))
                        .Where(p => p.visibility != false)
                .Include(p => p.Photos).ToListAsync();
                    }
                    else
                    {
                        if (parameter == "Description")
                        {
                            products = await _context.Products.Where(p => p.Product_Discription.ToLower().Contains(searchQuery))
                            .Where(p => p.visibility != false)
                .Include(p => p.Photos).ToListAsync();
                        }
                        else
                        {
                            if (parameter == "All")
                            {
                                products = await _context.Products.Where(p => p.Product_Discription.ToLower().Contains(searchQuery)
                                || p.Product_name.ToLower().Contains(searchQuery)
                                || p.Category.ToLower().Contains(searchQuery)
                                || p.Sub_category.ToLower().Contains(searchQuery)
                                )
                                .Where(p => p.visibility != false)
                .Include(p => p.Photos).ToListAsync();
                            }
                        }
                    }
                }
            }

            return products;
        }

        public async Task<ICollection<Product>> GetProductsByNameAndSubCategory( int productId)
        {
            List<Product> products = null;
            var product = await GetProduct(productId);
            

            products = await _context.Products.Where(p => p.Sub_category.ToLower().Contains(product.subCategory.SubCategoryName.ToLower()))
            .Where(q => q.Product_name.ToLower().Equals(product.Product_name.ToLower())).Where(p => p.visibility != false)
            .Include(p => p.Photos).ToListAsync();

            return products;
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Photos).Include(p => p.Ratings).FirstOrDefaultAsync(i => i.Id == id);

            return product;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateDeliveryInfo(DeliveryInfo DeliveryInfo)
        {
            var prod = await _context.DeliveryInfo.FindAsync(DeliveryInfo.Id);
            if (prod == null)
                throw new AppException("DeliveryInfo not found");

            prod.FName = DeliveryInfo.FName;
            prod.Address = DeliveryInfo.Address;
            prod.District = DeliveryInfo.District;
            prod.City = DeliveryInfo.City;
            prod.MobileNumber = DeliveryInfo.MobileNumber;

            _context.DeliveryInfo.Update(prod);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DeliveryInfo> GetDeliveryInfo(int id)
        {
            var info = await _context.DeliveryInfo.FirstOrDefaultAsync(i => i.Id == id);

            return info;
        }

        public async Task<ICollection<DeliveryInfo>> GetDeliveryInfosOfUser(string userId)
        {
            var info = await _context.DeliveryInfo.Where(i => i.UserId == userId).ToListAsync();

            return info;
        }

        public async Task<DeliveryInfo> GetDeliveryInfoOfDefault(string userId)
        {

            var info = await _context.DeliveryInfo.Where(i => i.UserId == userId)
            .FirstOrDefaultAsync(i => i.isDefault == true);

            return info;
        }

        public async Task<DeliveryInfo> SetAlternateDefault(string userId)
        {
            var info = await _context.DeliveryInfo.Where(i => i.UserId == userId).FirstOrDefaultAsync(i => i.isDefault == false);

            return info;
        }

        public async Task<CartProduct> GetCartProduct(int CartproductId)
        {
            var CartProduct = await _context.CartProduct.FirstOrDefaultAsync(p => p.Id == CartproductId);

            return CartProduct;
        }

        public async Task<ICollection<CartProduct>> GetAllCartProduct(int CartId)
        {
            var cartProducts = await _context.CartProduct.
            Where(i => i.CartId == CartId).ToListAsync();

            return cartProducts;
        }

        public async Task UpdateCartDetails(CartProduct CartProduct)
        {
            var cartProducts = await _context.CartProduct.FindAsync(CartProduct.Id);
            if (cartProducts == null)
                throw new AppException("Product is not avilable ");

            cartProducts.Count = CartProduct.Count;
            cartProducts.Price = CartProduct.Price;

            _context.CartProduct.Update(cartProducts);
        }

        public async Task<CartProduct> FindProductMatchInCart(int productId, int cartId)
        {

            var prod = await _context.CartProduct.Where(p => p.ProductId == productId).FirstOrDefaultAsync(p => p.CartId == cartId);

            return prod;
        }

        public async Task<bool> UpdateRating(Rating rating)
        {
            var rate = await _context.Ratings.FindAsync(rating.Id);
            if (rating == null)
                throw new AppException("rating not available");


            rate.RatingValue = rating.RatingValue;
            rate.Comment = rating.Comment;

            _context.Ratings.Update(rate);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Rating> GetRatingById(Rating rating)
        {
            var rate = await _context.Ratings.FindAsync(rating.Id);

            if (rate == null)
                throw new AppException("Rating Not Found");

            return rate;
        }

        public async Task<PhotoForUser> GetPhotoOfUser(string UserId)
        {
            var photo = await _context.PhotoForUsers.FirstOrDefaultAsync(p => p.UserId == UserId);

            return photo;
        }

        public async Task<BillingInfo> GetBillingInfo(int id)
        {
            var info = await _context.BillingInfo.FirstOrDefaultAsync(i => i.Id == id);

            return info;
        }

        public async Task<BillingInfo> GetBillingInfoOfDefault(string userId)
        {
            var info = await _context.BillingInfo.Where(i => i.UserId == userId)
           .FirstOrDefaultAsync(i => i.isDefault == true);

            return info;
        }

        public async Task<BillingInfo> AlternateDefault(string userId)
        {
            var info = await _context.BillingInfo.Where(i => i.UserId == userId).FirstOrDefaultAsync(i => i.isDefault == false);
            return info;
        }

        public async Task<ICollection<BillingInfo>> GetBillingInfosOfUser(string userId)
        {
            var info = await _context.BillingInfo.Where(i => i.UserId == userId).ToListAsync();
            return info;
        }

        public async Task<bool> UpdateBillingInfo(BillingInfo BillingInfo)
        {
            var billinginfo = await _context.BillingInfo.FindAsync(BillingInfo.Id);
            if (billinginfo == null)
                throw new AppException("Product is not avilable ");

            billinginfo.FName = BillingInfo.FName;
            billinginfo.District = BillingInfo.District;
            billinginfo.MobileNumber = BillingInfo.MobileNumber;
            billinginfo.Address = BillingInfo.Address;
            billinginfo.City = BillingInfo.City;
            billinginfo.OTP = BillingInfo.OTP;
            billinginfo.isOTP = BillingInfo.isOTP;
            billinginfo.isMobileVerfied = BillingInfo.isMobileVerfied;
            //    if(billinginfo.isMobileVerfied)
            //         billinginfo.isMobileVerfied=false;

            _context.BillingInfo.Update(billinginfo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<BillingInfo> GetBillingInfobyOtp(string userId, string otp)
        {
            var info = await _context.BillingInfo.Where(i => i.UserId == userId).FirstOrDefaultAsync(q => q.OTP == otp);
            return info;
        }

        public async Task<Order> GetOrder(int id)
        {
            var order = await _context.Orders.Include(p => p.orderDetails)
          .FirstOrDefaultAsync(i => i.Id == id);

            return order;
        }

        public void AddOrder(Order Order)
        {
            _context.Orders.Add(Order);
            _context.SaveChanges();
        }

        public async Task<bool> UpdateOrder(Order Order)
        {
            var Orderinfo = await _context.Orders.FindAsync(Order.Id);

            if (Orderinfo == null)
                throw new AppException("Orderinfo is not avilable ");

            Orderinfo.PaymentStatus = Order.PaymentStatus;
            Orderinfo.Total_Price = Order.Total_Price;

            _context.Orders.Update(Orderinfo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSellerInfo(SellerPaymentInfo SellerPaymentInfo)
        {

            var sellerinfo = await _context.SellerPaymentInfo.FindAsync(SellerPaymentInfo.Id);

            if (sellerinfo == null)
                throw new AppException("sellerPaymentInfo is not avilable ");

            var id = sellerinfo.Id;
            sellerinfo = SellerPaymentInfo;
            sellerinfo.Id = id;

            _context.SellerPaymentInfo.Update(sellerinfo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateBuyerInfo(BuyerPaymentInfo BuyerPaymentInfo)
        {

            var buyerinfo = await _context.BuyerPaymentInfo.FindAsync(BuyerPaymentInfo.Id);

            if (buyerinfo == null)
                throw new AppException("sellerPaymentInfo is not avilable ");

            var id = buyerinfo.Id;
            buyerinfo = BuyerPaymentInfo;
            buyerinfo.Id = id;

            _context.BuyerPaymentInfo.Update(buyerinfo);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}