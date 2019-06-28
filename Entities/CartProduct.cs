namespace pro.backend.Entities
{
    public class CartProduct
    {
        public int  Id { get; set; }
        public int ProductId {get;set;}
        public string product_Name { get; set; }
        public string MainPhotoUrl { get; set; }
        public float Price { get; set; }
        public int  Count { get; set; }
         public int CartId { get; set; }
         public Cart cart { get; set; }
    }
}