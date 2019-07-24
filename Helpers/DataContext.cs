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
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Advertisement>()
            
            .HasOne(p => p.PhotoForAd)
            .WithOne(i => i.Advertisement)
            .HasForeignKey<PhotoForAd>(b => b.AdId);
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //Adding Configerations
            // builder.Entity<UserRole>(userRole=>
            //  {
            //     userRole.HasKey(ur=> new {ur.UserId,ur.RoleId});//Sets the properties that make up the primary key for this entity type.
            //     userRole.HasOne(ur =>ur.user) 
            //     .WithMany(r => r.UserRoles)     
            //     .HasForeignKey(ur =>ur.RoleId)  
            //     .IsRequired();                         
            //     userRole.HasOne(ur =>ur.user) 
            //     .WithMany(r => r.UserRoles)     
            //     .HasForeignKey(ur =>ur.UserId)  
            //     .IsRequired();   
            // });
        }

    }


}