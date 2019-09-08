using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.Entities;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Services
{
    public class ProductService : iProductService
    {
        private DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }
        public void AddProduct(Product product)
        {

            _context.Products.Add(product);
            _context.SaveChanges();

        }
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products;
        }
        public Product GetById(int id)
        {
            return _context.Products.Find(id);
        }
        public void DeleteProduct(int id)
        {

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
        public async Task<bool> UpdateProduct(Product product)
        {

            var prod = await _context.Products.FindAsync(product.Id);

            if (prod == null)
                throw new AppException("Product not found");

            // update user properties
            prod.Product_name = product.Product_name;
            prod.rating = prod.rating;
            prod.Quantity = product.Quantity;
            prod.ReorderLevel = product.ReorderLevel;
            prod.Price = product.Price;
            prod.Product_Discription = product.Product_Discription;
            // prod.Category = product.Category;
            // prod.Sub_category = product.Sub_category;
            // prod.CategoryId = product.CategoryId;
            // prod.Sub_categoryId = product.Sub_categoryId;
            if (prod.visibility)
            {
                prod.visibility = product.visibility;
            }
            _context.Products.Update(prod);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Product>> TopSelling(){

            var products = await _context.Products.Where(p => p.visibility != false && p.NumberOfSales>0).Include(p => p.Photos).ToListAsync();
            var ordered = products.OrderByDescending(i => i.NumberOfSales);

            IEnumerable<Product> values;

            if(ordered.Count()>6)
            values = ordered.Take(6);
            else
            values = ordered.Take(ordered.Count());

            return values;
        }
    }
}