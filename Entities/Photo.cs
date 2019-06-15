using System;

namespace pro.backend.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool isMain { get; set; }
        public  string PublicID { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}