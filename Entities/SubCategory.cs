using System.Collections.Generic;

namespace pro.backend.Entities
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }
        public string url { get; set; }
        public ICollection<Product> Products { get; set; }
        public PhotoForCategory PhotoForCategory { get; set; }
        public Category category { get; set; }
        
    }
}