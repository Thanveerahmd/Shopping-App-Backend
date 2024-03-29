
using System;
using pro.backend.Entities;

namespace pro.backend.Dtos
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string ReceiverEmail { get; set; }
        public string Message { get; set; }
        public bool isUnRead { get; set; }
        public DateTime  TimeSent { get; set; }
        public string  UserMail { get; set; }
        public string  UserFullName { get; set; }
        public ChatDto()
        {
            TimeSent = DateTime.Now;
        }
    }
}