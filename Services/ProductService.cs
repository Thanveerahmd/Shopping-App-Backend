using pro.backend.Entities;
using Project.Helpers;
using System.Collections.Generic;

namespace pro.backend.Services
{
    public class ProductService : iProductService
    {
         private DataContext _context;

        public  ProductService(DataContext context)
        {
            _context = context;
        }
        public void AddProduct(Product product){

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

        public void DeleteProduct(int id){

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
        // public IEnumerable<ProductViewModel2> getProduct() => 
        //     _context.Products.ToList().Select(x => new ProductViewModel2{
        //         id = x.id,
        //         name = x.name,
        //         quantity = x.quantity,
        //         reorderLevel = x.reorderLevel,
        //         price = x.price.ToString("N2"), // 1100.50 => 1,100.50
        //         description = x.description,
        //         category = x.category,
        //         sub_category = x.sub_category
        //     });
        
        public void UpdateProduct(Product product){

            var prod = _context.Products.Find(product.Id);

            if (prod == null)
                throw new AppException("Product not found");

            // update user properties
            prod.Id = product.Id;
            prod.Product_name = product.Product_name;
            prod.Quantity = product.Quantity;
            prod.ReorderLevel = product.ReorderLevel;
            prod.Price = product.Price;
            prod.Product_Discription = product.Product_Discription;
            prod.Category = product.Category;
            prod.Sub_category = product.Sub_category;

            _context.Products.Update(product);
            _context.SaveChanges();
        }
    }
}