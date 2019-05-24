using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Project.Dtos;
using pro.backend.Entities;

namespace pro.backend.Services
{
    public interface iProductService
    {
          Task createProduct(ProductViewModel vm );
          IEnumerable<ProductViewModel2> getProduct();
          Task<Response> updateProduct(Request request);

    }
}