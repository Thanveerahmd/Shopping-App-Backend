using System;
using System.Collections.Generic;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SellerId { get; set; }
        public string  Product_name { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public float  Price { get; set; }
        public string  Product_Discription { get; set; }
        public string Category { get; set; }
        public string Sub_category { get; set; }
        public string PhotoUrl { get; set; }  
        public bool visibility {get;set;}
        public ICollection<PhotoDto> Photos { get; set; }
        public ICollection<Rating> Ratings {get;set;}
     
        
    }
}