using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWP391_DEMO.Models
{
    public partial class SWP391_DEMOContext : DbContext
    {
        public SWP391_DEMOContext()
        {
        }

        public SWP391_DEMOContext(DbContextOptions<SWP391_DEMOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductAnalytic> ProductAnalytics { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserVoucher> UserVouchers { get; set; } = null!;
        public virtual DbSet<Voucher> Vouchers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=PANHVU04\\SQLEXPRESS;User ID=sa;Password=12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Database=SWP391_DEMO");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("carts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__carts__user_id__6C190EBB");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.CartId, e.ProductId })
                    .HasName("PK__cart_ite__6A850DF8145AC1EC");

                entity.ToTable("cart_items");

                entity.Property(e => e.CartId).HasColumnName("cart_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__cart_item__cart___6EF57B66");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__cart_item__produ__6FE99F9F");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Index).HasColumnName("index");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__customer__B9BE370F391166AB");

                entity.ToTable("customers");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(255)
                    .HasColumnName("phoneNumber");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__customers__user___5070F446");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(255)
                    .HasColumnName("phone_number");

                entity.Property(e => e.ShippingFee)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("shipping_fee");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("total_price");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__orders__user_id__6477ECF3");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK__orders__voucher___656C112C");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PK__order_it__022945F61A21DFEB");

                entity.ToTable("order_items");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.ItemPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("item_price");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("product_name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("unit_price");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__order_ite__order__68487DD7");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__order_ite__produ__693CA210");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__products__catego__5535A963");
            });

            modelBuilder.Entity<ProductAnalytic>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK__product___47027DF51D17F478");

                entity.ToTable("product_analytics");

                entity.Property(e => e.ProductId)
                    .ValueGeneratedNever()
                    .HasColumnName("product_id");

                entity.Property(e => e.Purchases).HasColumnName("purchases");

                entity.Property(e => e.Views).HasColumnName("views");

                entity.HasOne(d => d.Product)
                    .WithOne(p => p.ProductAnalytic)
                    .HasForeignKey<ProductAnalytic>(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__5AEE82B9");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("image_url");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__product_i__produ__5812160E");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "UQ__users__AB6E6164AAA6E7E5")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__users__role_id__4D94879B");
            });

            modelBuilder.Entity<UserVoucher>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.VoucherId })
                    .HasName("PK__user_vou__21B558F5D5331FF8");

                entity.ToTable("user_vouchers");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserVouchers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_vouc__user___5FB337D6");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.UserVouchers)
                    .HasForeignKey(d => d.VoucherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_vouc__vouch__60A75C0F");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("vouchers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(255)
                    .HasColumnName("code");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("discount_amount");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.MinimumOrderAmount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("minimum_order_amount");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.StartDate).HasColumnName("start_date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
