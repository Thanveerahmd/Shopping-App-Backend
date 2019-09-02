using System;

namespace pro.backend.Entities
{
    public class PhotoForAd
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public string UserId { get; set; }
        public int AdId { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}