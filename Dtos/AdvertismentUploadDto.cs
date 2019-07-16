using System;
using Microsoft.AspNetCore.Http;

namespace pro.backend.Dtos
{
    public class AdvertismentUploadDto
    {
        public string Url { get; set; }
        public IFormFile file { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public AdvertismentUploadDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}