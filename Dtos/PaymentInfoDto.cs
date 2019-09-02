using System;

namespace pro.backend.Dtos
{
    public class PaymentInfoDto
    {
        public int Id { get; set; }
        public int order_id { get; set; }
        public long payment_id { get; set; }
        public float payhere_amount { get; set; }
        public string payhere_currency { get; set; }
        public int status_code { get; set; }
        public string method { get; set; }
        public string status_message { get; set; }
        public string card_holder_name { get; set; }
        public string card_no { get; set; }
        public string card_expiry { get; set; }
        public DateTime DateOfPayment { get; set; }

        public PaymentInfoDto()
        {
            DateOfPayment = DateTime.Now;
        }
    }
}