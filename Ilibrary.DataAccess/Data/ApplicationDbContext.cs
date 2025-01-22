using Ilibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Type = Ilibrary.Models.Type;



namespace Ilibrary.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }

        public DbSet<Section> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Section> Section { get; set; }
        
        public DbSet<Type> Type { get; set; }
        public DbSet<Sizes> Sizes { get; set; }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Section>().HasData(
           new Section { Id = 1, Name = "Upper" },
           new Section { Id = 2, Name = "Lower" },
           new Section { Id = 3, Name = "Footwear" }
            );
            modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Tech Solution",
                StreetAddress = "123 Tech St",
                City = "Tech City",
                PostalCode = "12121",
                State = "IL",
                PhoneNumber = "6669990000"
            },
new Company
{
    Id = 2,
    Name = "Vivid Books",

    StreetAddress = "999 id St",
    City = "Vid City",
    PostalCode = "66666",
    State = "IL",
    PhoneNumber = "7779990000"
},
new Company
{
    Id = 3,
    Name = "Readers Club",
    StreetAddress = "999 Main St",
    City = "Lala land",
    PostalCode = "99999",
    State = "NY",
    PhoneNumber = "1113335555"
});

        }
    }

}  

