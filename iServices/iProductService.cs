using pro.backend.Entities;
using System.Collections.Generic;

namespace pro.backend.Services
{
    public interface iProductService
    {
        void AddProduct(Product product);
        IEnumerable<Product> GetAllProducts();
        Product GetById(int id);
        void DeleteProduct(int id);
        void UpdateProduct(Product product);

    }
}