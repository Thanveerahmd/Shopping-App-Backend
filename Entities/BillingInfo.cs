namespace pro.backend.Entities
{
    public class BillingInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FName { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string MobileNumber { get; set; }
        public bool isMobileVerfied {get;set;}
        public string OTP {get;set;}
        public bool isOTP {get;set;}
        public bool isDefault { get; set; }

        public int OTPCount {get;set;}
    }
}