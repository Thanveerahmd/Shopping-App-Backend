
using System;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class PromoScoreDto
    {
        public int Id { get; set; }
        public string Promotion_Name { get; set; }
        public int ProductId { get; set; }
        public string Promotion_Description { get; set; }   

        public int Score {get;set;}
        
    }
}