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

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _context.Products.Include(p => p.Photos).ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> GetAllProductsOfSeller(string sellerID) //have to change
        {
            var products = await _context.Products.Where(p => p.SellerId == sellerID)
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
                .Include(p => p.Photos).ToListAsync();
            }
            else
            {
                if (parameter == "Category")
                {
                    products = await _context.Products.Where(p => p.Category.ToLower().Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
                }
                else
                {
                    if (parameter == "Name")
                    {
                        products = await _context.Products.Where(p => p.Product_name.ToLower().Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
                    }
                    else
                    {
                        if (parameter == "Description")
                        {
                            products = await _context.Products.Where(p => p.Product_Discription.ToLower().Contains(searchQuery))
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
                .Include(p => p.Photos).ToListAsync();
                            }
                        }
                    }
                }
            }

            return products;
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Photos).FirstOrDefaultAsync(i => i.Id == id);

            return product;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async void UpdateDeliveryInfo(DeliveryInfo DeliveryInfo)
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
            await _context.SaveChangesAsync();
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

        public async void UpdateCartDetails(CartProduct CartProduct)
        {
            var cartProducts = await _context.CartProduct.FindAsync(CartProduct.Id);
            if (cartProducts == null)
                throw new AppException("Product is not avilable ");

            cartProducts.Count=CartProduct.Count;
            cartProducts.Price=CartProduct.Price;
            
            _context.CartProduct.Update(cartProducts);
            await _context.SaveChangesAsync();
        }

        public async Task<PhotoForUser> GetPhotoOfUser(string UserId)
        {
           var photo = await _context.PhotoForUsers.FirstOrDefaultAsync(p => p.UserId==UserId);

            return photo;
        }
    }
}