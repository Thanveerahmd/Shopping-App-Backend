﻿namespace pro.backend.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string UserId { get; set; }
        public string StoreName { get; set; }
    }
}