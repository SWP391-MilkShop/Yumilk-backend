using Microsoft.EntityFrameworkCore;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Data {
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions options) : base(options) {
        }

        public AppDbContext() {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json").Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
        DbSet<User> User {  get; set; }
        DbSet<Role> Role { get; set; }
        DbSet<RefreshToken> RefreshToken { get; set; }
        DbSet<Customer> Customer { get; set; }
        DbSet<CustomerAddress> CustomerAddress { get; set; }
        DbSet<Product> Product { get; set; }
        DbSet<Category> Category { get; set; }
        DbSet<Unit> Unit { get; set; }
        DbSet<Brand> Brand { get; set; }
        DbSet<ProductAttribute> ProductAttribute { get; set; }
        DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }     
    }
}
