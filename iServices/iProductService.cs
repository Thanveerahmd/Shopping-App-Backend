using pro.backend.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pro.backend.Services
{
    public interface iProductService
    {
        void AddProduct(Product product);
        IEnumerable<Product> GetAllProducts();
        Product GetById(int id);
        void DeleteProduct(int id);
        Task<bool> UpdateProduct(Product product);
        Task<IEnumerable<Product>> TopSelling();

    }
}