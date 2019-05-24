using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Dtos;
using Project.Entities;
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
    }
}