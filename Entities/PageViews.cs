using System;

namespace pro.backend.Entities
{
    public class PageViews
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Sub_category { get; set; }
        public int Sub_categoryId { get; set; }
        public int NoOfVisits { get; set; }
        public DateTime LatestVisit { get; set; }
    }
}