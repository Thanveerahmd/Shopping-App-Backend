namespace pro.backend.Entities
{
    public class Product
    {
        //public int id { get; set; }
        public string  Product_name { get; set; }

        public int Quantity { get; set; }

        public int ReorderLevel { get; set; }

        public float  Price { get; set; }

        public string  Product_Discription { get; set; }

        public string Category { get; set; }

        public string Sub_category { get; set; }
    }
    public class ProductViewModel{
        public int id { get; set; }
        public string name { get; set; }

        public int quantity { get; set; }

        public int reorderLevel { get; set; }

        public int price { get; set; }

        public string description { get; set; }
        public string category { get; set; }

        public string sub_category { get; set; }
    
    }
    public class ProductViewModel2{
        public int id { get; set; }
        public string name { get; set; }

        public int quantity { get; set; }

        public int reorderLevel { get; set; }

        public string price { get; set; }

        public string description { get; set; }
        public string category { get; set; }

        public string sub_category { get; set; }
    
    }

    public class Request{
        public int id { get; set; }
        public string name { get; set; }

        public int quantity { get; set; }

        public int reorderLevel { get; set; }

        public string price { get; set; }

        public string description { get; set; }

        public string category { get; set; }

        public string sub_category { get; set; }
    }
    public class Response{
        public int id { get; set; }
        public string name { get; set; }

        public int quantity { get; set; }

        public int reorderLevel { get; set; }

        public string price { get; set; }

        public string description { get; set; }

        public string category { get; set; }

        public string sub_category { get; set; }
    }
}