using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Entities;


namespace  Project.Helpers
{
    public class DataContext : IdentityDbContext<User,Role,string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        

        

        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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