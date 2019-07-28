using System;
using System.Collections.Generic;
using pro.backend.Entities;
using pro.backend.iServices;
using pro.backend.Services;

namespace pro.backend.Dtos
{
    public class OrderInfoForSellerDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string product_Name { get; set; }
        public string MainPhotoUrl { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public DeliveryInfo deliveryInfo { get; set; }
        public int OrderId { get; set; }
        public string sellerId { get; set; }

    }

}