using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Entities;
using pro.backend.Entities;


namespace Project.Helpers
{
    public class DataContext : IdentityDbContext<User, Role, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartProduct> CartProduct { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<DeliveryInfo> DeliveryInfo { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<PhotoForUser> PhotoForUsers { get; set; }
        public DbSet<BillingInfo> BillingInfo { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<SellerPaymentInfo> SellerPaymentInfo { get; set; }
        public DbSet<BuyerPaymentInfo> BuyerPaymentInfo { get; set; }
        public DbSet<orderDetails> orderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Advertisement> Advertisement { get; set; }
        public DbSet<PhotoForAd> PhotoForAd { get; set; }
        public DbSet<DeviceToken> DeviceToken{ get; set; }
        public DbSet<Chat> Chat{ get; set; }

        public DbSet<Promo> Promo{ get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Advertisement>()
            
            .HasOne(p => p.PhotoForAd)
            .WithOne(i => i.Advertisement)
            .HasForeignKey<PhotoForAd>(b => b.AdId);

            builder.Entity<Promo>()
            .Property<string>("DayCollection")
            .HasField("_Days_of_The_Week");
            
        }

    }


}