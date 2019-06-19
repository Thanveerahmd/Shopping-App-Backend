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

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _context.Products.Include(p => p.Photos).ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> GetAllProductsOfSeller(string sellerID) //have to change
        {
           var products = await _context.Products.Where(p =>p.SellerId == sellerID)
           .Include(p => p.Photos).ToListAsync();

            return products;
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

        public async Task<IEnumerable<Product>> GetProductsBySearchQuery(string searchQuery,string parameter)
        {
            List <Product> products = null;

            if(parameter == "Sub Category"){
                products = await _context.Products.Where(p =>p.Sub_category.Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
            }else{
                if(parameter == "Category"){
                    products = await _context.Products.Where(p =>p.Category.Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
                }else{
                    if(parameter == "Name"){
                        products = await _context.Products.Where(p =>p.Product_name.Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
                    }else{
                        if(parameter == "Description"){
                            products = await _context.Products.Where(p =>p.Product_Discription.Contains(searchQuery))
                .Include(p => p.Photos).ToListAsync();
                        }else{
                            if(parameter == "All"){
                                products = await _context.Products.Where(p =>p.Product_Discription.Contains(searchQuery) 
                                || p.Product_name.Contains(searchQuery)
                                || p.Category.Contains(searchQuery)
                                || p.Sub_category.Contains(searchQuery)
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
    }
}