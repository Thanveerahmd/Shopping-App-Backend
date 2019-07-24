using System;

namespace pro.backend.Entities
{
    public class DeviceToken
    {
        public int id { get; set; }
        public string DeviceId { get; set; }
        public string FirebaseToken { get; set; }
        public double Last_Lat { get; set; }
        public double Last_Lng { get; set; }
        public DateTime LastNotifyTime { get; set; }
    }
}