using System;
using System.Collections.Generic;

namespace pro.backend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public float Total_Price { get; set; }
        public DateTime DateAdded { get; set; }
        public string PaymentStatus { get; set; }
        public DeliveryInfo DeliveryInfo { get; set; }
        public ICollection<orderDetails> orderDetails { get; set; }
    }
}