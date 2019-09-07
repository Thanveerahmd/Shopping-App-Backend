using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro.backend.Entities
{
    public class ProductView
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int NoOfVisits { get; set; }
        public DateTime LatestVisit { get; set; }
    }
}