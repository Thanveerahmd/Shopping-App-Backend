using System.Collections.Generic;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class CategoryReturnDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public ICollection<SubCategoryDto> SubCategorys { get; set; }


    }
}