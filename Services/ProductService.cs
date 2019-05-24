using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Dtos;
using pro.backend.Entities;
using Project.Helpers;
using  Project.Services;

namespace pro.backend.Services
{
    public class ProductService : iProductService
    {
         private DataContext _context;

        public  ProductService(DataContext context)
        {
            _context = context;
        }
        public void addProduct(int id, string name, int quantity, int reorderLevel, float price, string description){
            _context.Products.Add(new Product
            {
                id = id,
                Product_name = name,
                Quantity = quantity,
                ReorderLevel = reorderLevel,
                Price = price,
                Product_Discription = description 
            });
        }
        public void getProduct(){
            _context.Products.ToList().Select(x => new ProductViewModel{
                id = x.id,
                name = x.name,
                quantity = x.quantity,
                reorderLevel = x.reorderLevel,
                price = x.price.ToString("N2"), // 1100.50 => 1,100.50
                description = x.description 
            });
        }
    }
    public class ProductViewModel{
        public string id { get; set; }
        public string name { get; set; }

        public int quantity { get; set; }

        public int reorderLevel { get; set; }

        public string price { get; set; }

        public string description { get; set; }
    
    }
}