using System;
using System.Collections.Generic;
using pro.backend.Entities;
namespace pro.backend.Dtos
{
    public class OrderDetailsForUserPreference
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string BuyerId { get; set; }
        public string product_Name { get; set; }
        public float Price { get; set; }
        public int quantity { get; set; }
        public int OrderId { get; set; }
        public string sellerId { get; set; }

    }

}