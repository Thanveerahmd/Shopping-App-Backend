using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class OrderProductDto
    {
       
        public int ProductId { get; set; }
        public string product_Name { get; set; }
        public string MainPhotoUrl { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        
    }
}