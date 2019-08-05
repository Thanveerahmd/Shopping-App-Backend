
namespace pro.backend.Dtos
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }
        public bool isProductAvilable { get; set; }
        public string url { get; set; }
      
    }
}