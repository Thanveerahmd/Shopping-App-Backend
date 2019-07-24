using System;

namespace pro.backend.Dtos
{
    public class DeviceDetailsDto
    {
        public int id { get; set; }
        public string DeviceId { get; set; }
        public string FirebaseToken { get; set; }
        public double Last_Lat { get; set; }
        public double Last_Lng { get; set; }
        public DateTime LastNotifyTime { get; set; }
    }
}