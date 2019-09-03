using System;

namespace pro.backend.Entities
{
    public class BuyerSearch
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Keyword { get; set; }
        public int NoOfSearch { get; set; }
        public DateTime LatestVisit { get; set; }
    }
}