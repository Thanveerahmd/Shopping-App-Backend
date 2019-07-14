using System;
using Project.Entities;

namespace pro.backend.Entities
{
    public class Advertisement
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public string UserId { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public User user { get; set; }
    }
}