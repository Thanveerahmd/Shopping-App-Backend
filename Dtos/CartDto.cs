using System.Collections.Generic;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class CartDto
    {
        public int CartId { get; set; }
        public string BuyerId { get; set; }
        public ICollection<CartProduct> CartDetails { get; set; }
        public float Total_Price { get; set; }
    }
}