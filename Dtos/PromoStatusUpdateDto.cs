
using System;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class PromoStatusUpdateDto
    {
        public int Id { get; set; }
       
        public string Reason { get; set; }

        public string Status { get; set; }
        
    }
}