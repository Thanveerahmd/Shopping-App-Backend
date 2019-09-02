namespace pro.backend.Dtos
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Product_name { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public float Price { get; set; }
        public string Product_Discription { get; set; }
        public string Category { get; set; }
        public string Sub_category { get; set; }
        public string PhotoUrl { get; set; }
        public bool visibility { get; set; }
        public float rating { get; set; }
    }
}