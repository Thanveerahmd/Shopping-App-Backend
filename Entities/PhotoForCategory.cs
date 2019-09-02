using System;
using Project.Entities;

namespace pro.backend.Entities
{
    public class PhotoForCategory
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicID { get; set; }
        public int SubCategoryId { get; set; }
        public SubCategory subCategory { get; set; }
    }
}