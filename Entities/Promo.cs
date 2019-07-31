using System;
using System.Collections.Generic;
using Project.Entities;


namespace pro.backend.Entities
{
    public class Promo
    {
        public int Id { get; set; }
       
        public string Promotion_Name { get; set; }

        public int ProductId { get; set; }

        public string Promotion_Description { get; set; }

        public string[] Day_of_The_Week { get; set; }

        public int Frequency { get; set; }

        public string Status { get; set; }

        public string UserId { get; set; }

        public User user { get; set; }


    
    }

   
}