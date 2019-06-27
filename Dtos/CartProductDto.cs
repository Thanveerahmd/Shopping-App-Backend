using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class CartProductDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string product_Name { get; set; }
        public int productId {get;set;}
        public string MainPhotoUrl { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public string Colour { get; set; }
        public string Option { get; set; }
        public int CartId { get; set; }
        public Cart cart { get; set; }
    }
}