using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Project.Dtos;
using pro.backend.Entities;

namespace pro.backend.Services
{
    public interface iProductService
    {
        Product AddProduct(Product product);
        void DeleteProduct(int id);
        void UpdateProduct(Product product);

    }
}