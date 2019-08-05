using System.Collections.Generic;

namespace pro.backend.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public ICollection<SubCategory> SubCategorys { get; set; }

    }
}