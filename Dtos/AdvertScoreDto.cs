using System;
using Microsoft.AspNetCore.Http;

namespace pro.backend.Dtos
{
    public class AdvertScoreDto
    {
       public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public string UserId { get; set; }
        public string PaymentStatus { get; set; }
        public int ProductId { get; set; }
        public int timestamp { get; set; }
        public string ActivationStatus { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string ProductSubCategory { get; set; }

        public float ProductPrice {get;set;}

        public int Score {get;set;}

    }
}