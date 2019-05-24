using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Dtos;
using pro.backend.Entities;
using Project.Helpers;
using Project.Services;

namespace pro.backend.Services
{
    public class ProductService : iProductService
    {
         private DataContext _context;

        public  ProductService(DataContext context)
        {
            _context = context;
        }
        public async Task createProduct(ProductViewModel vm ){
            _context.Products.Add(new Product
            {
                id = vm.id,
                Product_name = vm.name,
                Quantity = vm.quantity,
                ReorderLevel = vm.reorderLevel,
                Price = vm.price,
                Product_Discription = vm.description,
                Category = vm.category,
                Sub_category = vm.sub_category
            });

            await _context.SaveChangesAsync();
        }
        public IEnumerable<ProductViewModel2> getProduct() => 
            _context.Products.ToList().Select(x => new ProductViewModel2{
                id = x.id,
                name = x.name,
                quantity = x.quantity,
                reorderLevel = x.reorderLevel,
                price = x.price.ToString("N2"), // 1100.50 => 1,100.50
                description = x.description,
                category = x.category,
                sub_category = x.sub_category
            });
        
        public async Task<Response> updateProduct(Request request){
            var product = _context.Products.FirstOrDefault(x => x.id == request.id);
            
            product.name = request.name;
            product.quantity = request.quantity;
            product.reorderLevel = request.reorderLevel; 
            product.price = request.price;
            product.description = request.description;
            product.category = request.category; 
            product.sub_category = request.sub_category; 

            await _context.SaveChangesAsync();
            return new Response{
                id = product.id,
                name = product.name,
                quantity = product.quantity,
                reorderLevel = product.reorderLevel,
                price = product.price,
                description = product.description,
                category = product.category,
                sub_category = product.sub_category
            };
        }
    }
}