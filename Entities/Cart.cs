using System.Collections.Generic;

namespace pro.backend.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public ICollection<CartProduct> CartDetails { get; set; }
        public float Total_Price { get; set; }

    }
}