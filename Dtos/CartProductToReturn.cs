namespace pro.backend.Dtos
{
    public class CartProductToReturn
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string product_Name { get; set; }
        public string MainPhotoUrl { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }


    }
}