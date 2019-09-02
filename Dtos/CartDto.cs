using System.Collections.Generic;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public float Total_Price { get; set; }
        public ICollection<CartProductToReturn> CartDetails { get; set; }

    }
}