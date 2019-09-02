namespace pro.backend.Dtos
{
    public class DeliveryInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FName { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string MobileNumber { get; set; }
        public bool isDefault { get; set; }
    }
}