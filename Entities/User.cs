using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using pro.backend.Entities;

namespace Project.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public long? FacebookId { get; set; }
        public string GoogleId { get; set; }
        public string imageUrl { get; set; }
        public bool isLocked { get; set; }
        public ICollection<DeliveryInfo> DeliveryDetails { get; set; }
        public PhotoForUser Photo { get; set; }
        public ICollection<Store> StoreInfo { get; set; }
        public ICollection<BillingInfo> BillingInfo { get; set; }
        public ICollection<PageViews> PageViews { get; set; }
        public ICollection<BuyerSearch> BuyerSearch { get; set; }

    }
}