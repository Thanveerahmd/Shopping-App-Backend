using System;
using Microsoft.AspNetCore.Http;

namespace pro.backend.Dtos
{
    public class AdvertismentUploadDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public int ProductId { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }

        public int timestamp { get; set; }

        public  AdvertismentUploadDto()
        {
             DateAdded = DateTime.Now;
        }
    }
}