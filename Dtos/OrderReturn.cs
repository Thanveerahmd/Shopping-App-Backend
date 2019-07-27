using System;
using System.Collections.Generic;
using pro.backend.Entities;
namespace pro.backend.Dtos
{
    public class OrderReturnDto
    {
       
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public float Total_Price { get; set; }
        public DateTime DateAdded { get; set; }
        public string PaymentStatus { get; set; }
        public ICollection<OrderReturnIncludes> orderDetails { get; set; }
        
    }
}