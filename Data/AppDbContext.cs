using Microsoft.EntityFrameworkCore;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Data
{
    public class AppDbContext : DbContext
    {

        private readonly IConfiguration _configuration;
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartDetail> CartDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ProductAnalytic> ProductAnalytics { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<UserVoucher> UserVouchers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Username).HasColumnName("Username");
                entity.Property(e => e.Password).HasColumnName("Password");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.VerificationToken).HasColumnName("verification_token");
                entity.Property(e => e.AccessToken).HasColumnName("access_token");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId);

                entity.HasOne(e => e.Customer)
                    .WithOne()
                    .HasForeignKey<User>(e => e.Id);

                entity.HasMany(e => e.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(rt => rt.UserId);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnName("deleted_at");

                entity.HasMany(d => d.Users)
                    .WithOne(p => p.Role)
                    .HasForeignKey(d => d.RoleId);
            });
            modelBuilder.Entity("SWP391_DEMO.Entities.CartDetail", b =>
            {
                b.HasKey("CartId", "ProductId");
            });
            modelBuilder.Entity("SWP391_DEMO.Entities.OrderDetail", b =>
            {
                b.HasKey("OrderId", "ProductId");
            });
            modelBuilder.Entity("SWP391_DEMO.Entities.UserVoucher", b =>
            {
                b.HasKey("CustomerId", "VoucherId");
            });
            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAttributeValue", b =>
            {
                b.HasKey("ProductId", "AttributeId");
            });
        }
    }
}
