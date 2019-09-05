using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro.backend.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string SellerId { get; set; }
        public string Product_name { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public float Price { get; set; }
        public string Product_Discription { get; set; }
        public string Category { get; set; }
        public string Sub_category { get; set; }
        public int Sub_categoryId { get; set; }
        public int CategoryId { get; set; }
        public bool visibility { get; set; }
        public int NumberOfSales { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public float rating { get; set; }
        public SubCategory subCategory { get; set; }

    }

}