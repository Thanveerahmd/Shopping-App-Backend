
using System;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class PromoDto
    {
        public int Id { get; set; }
        public string Promotion_Name { get; set; }
        public int ProductId { get; set; }
        public string Promotion_Description { get; set; }
        public string[] Day_of_The_Week { get; set; }
        public int  Frequency { get; set; }
        
    }
}