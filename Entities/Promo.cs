using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Entities;


namespace pro.backend.Entities
{
    public class Promo
    {
        private static readonly char delimiter = ';';
        public int Id { get; set; }

        public string Promotion_Name { get; set; }

        public int ProductId { get; set; }

        public string Promotion_Description { get; set; }

        private string _Days_of_The_Week;

        [NotMapped]
        public string[] Day_of_The_Week
        {

            get { return _Days_of_The_Week.Split(delimiter); }
            
            set
            {
                _Days_of_The_Week = string.Join($"{delimiter}", value);
            }
        }

        public int Frequency { get; set; }

        public string Status { get; set; }

        public string UserId { get; set; }

        public User user { get; set; }



    }


}