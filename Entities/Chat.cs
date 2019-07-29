using System;
using System.Collections.Generic;

namespace pro.backend.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public float Receiver { get; set; }
        public string Message { get; set; }
        public bool isUnRead { get; set; }
        public DateTime  TimeSent { get; set; }
    
    }
}