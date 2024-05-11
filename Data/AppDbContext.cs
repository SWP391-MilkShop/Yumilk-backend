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
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity("SWP391_DEMO.Entities.Brand", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("brands");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Cart", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                     .HasColumnName("created_at")
                     .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<Guid?>("CustomerId")
                    .HasColumnName("customer_id")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("CustomerId");

                b.ToTable("carts");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.CartDetail", b => {
                b.Property<int>("CartId")
                    .HasColumnName("cart_id")
                    .HasColumnType("int");

                b.Property<Guid>("ProductId")
                    .HasColumnName("product_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<int?>("Quantity")
                    .HasColumnType("int");

                b.HasKey("CartId", "ProductId");

                b.HasIndex("ProductId");

                b.ToTable("cart_details");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Category", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("categories");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Customer", b => {
                b.Property<Guid>("UserId")
                    .HasColumnName("user_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Email")
                    .HasColumnType("nvarchar(255)");

                b.Property<string>("GoogleId")
                    .HasColumnName("google_id")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PhoneNumber")
                    .HasColumnName("phone_number")
                    .HasColumnType("nvarchar(20)");

                b.Property<int?>("Points")
                    .HasColumnType("int");

                b.Property<string>("ProfilePictureUrl")
                    .HasColumnName("profile_picture_url")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("UserId");

                b.ToTable("customers");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.CustomerAddress", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Address")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("PhoneNumber")
                    .HasColumnName("phone_number")
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid?>("UserId")
                    .HasColumnName("user_id")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("customer_addresses");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Order", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Address")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<Guid?>("CustomerId")
                    .HasColumnName("customer_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Note")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PhoneNumber")
                    .IsRequired()
                    .HasColumnName("phone_number")
                    .HasColumnType("nvarchar(20)");

                b.Property<decimal?>("ShippingFee")
                    .HasColumnName("shipping_fee")
                    .HasColumnType("decimal(18,2)");

                b.Property<int?>("StatusId")
                    .HasColumnName("status_id")
                    .HasColumnType("int");

                b.Property<decimal?>("TotalAmount")
                    .HasColumnName("total_amount")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal?>("TotalPrice")
                    .HasColumnName("total_price")
                    .HasColumnType("decimal(18,2)");

                b.Property<int?>("VoucherId")
                    .HasColumnName("voucher_id")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("CustomerId");

                b.HasIndex("StatusId");

                b.HasIndex("VoucherId");

                b.ToTable("orders");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.OrderDetail", b => {
                b.Property<Guid>("OrderId")
                    .HasColumnName("order_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("ProductId")
                    .HasColumnName("product_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<decimal?>("ItemPrice")
                    .HasColumnName("item_price")
                    .HasColumnType("decimal(18,2)");

                b.Property<string>("ProductName")
                    .HasColumnName("product_name")
                    .HasColumnType("nvarchar(255)");

                b.Property<int?>("Quantity")
                    .HasColumnType("int");

                b.Property<decimal?>("UnitPrice")
                    .HasColumnName("unit_price")
                    .HasColumnType("decimal(18,2)");

                b.HasKey("OrderId", "ProductId");

                b.HasIndex("ProductId");

                b.ToTable("order_details");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.OrderStatus", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("order_statuses");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Product", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<int?>("BrandId")
                    .HasColumnName("brand_id")
                    .HasColumnType("int");

                b.Property<int?>("CategoryId")
                    .HasColumnName("category_id")
                    .HasColumnType("int");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(255)");

                b.Property<decimal?>("OriginalPrice")
                    .HasColumnName("original_price")
                    .HasColumnType("decimal(18,2)");

                b.Property<int?>("Quantity")
                    .HasColumnType("int");

                b.Property<decimal?>("SalePrice")
                    .HasColumnName("sale_price")
                    .HasColumnType("decimal(18,2)");

                b.Property<int?>("UnitId")
                    .HasColumnName("unit_id")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("BrandId");

                b.HasIndex("CategoryId");

                b.HasIndex("UnitId");

                b.ToTable("products");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAnalytic", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<Guid?>("ProductId")
                    .HasColumnName("product_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<int?>("PurchaseCount")
                    .HasColumnName("purchase_count")
                    .HasColumnType("int");

                b.Property<int?>("ViewCount")
                    .HasColumnName("view_count")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("ProductId");

                b.ToTable("product_analytics");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAttribute", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("product_attributes");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAttributeValue", b => {
                b.Property<Guid>("ProductId")
                    .HasColumnName("product_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("AttributeId")
                    .HasColumnName("attribute_id")
                    .HasColumnType("int");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar(2000)");

                b.HasKey("ProductId", "AttributeId");

                b.HasIndex("AttributeId");

                b.ToTable("product_attribute_values");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductImage", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("ImageUrl")
                    .IsRequired()
                    .HasColumnName("image_url")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<Guid?>("ProductId")
                    .HasColumnName("product_id")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("ProductId");

                b.ToTable("product_images");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.RefreshToken", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("Expires")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Token")
                    .HasColumnType("nvarchar(255)");

                b.Property<Guid?>("UserId")
                    .HasColumnName("user_id")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("refresh_tokens");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Role", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnName("created_at")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("roles");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Unit", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime?>("CreatedAt")
                   .HasColumnName("created_at")
                   .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("units");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.User", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("AccessToken")
                    .HasColumnName("access_token")
                    .HasColumnType("nvarchar(255)");

                b.Property<DateTime?>("CreatedAt")
                   .HasColumnName("created_at")
                   .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("FirstName")
                    .HasColumnName("first_name")
                    .HasColumnType("nvarchar(50)");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<string>("LastName")
                     .HasColumnName("last_name")
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("Password")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                b.Property<int?>("RoleId")
                    .HasColumnName("role_id")
                    .HasColumnType("int");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("VerificationToken")
                    .HasColumnName("verification_token")
                    .HasColumnType("nvarchar(6)");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("users");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.UserVoucher", b => {
                b.Property<Guid>("CustomerId")
                    .HasColumnName("customer_id")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("VoucherId")
                    .HasColumnName("voucher_id")
                    .HasColumnType("int");

                b.Property<DateTime?>("CreatedAt")
                   .HasColumnName("created_at")
                   .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.HasKey("CustomerId", "VoucherId");

                b.HasIndex("VoucherId");

                b.ToTable("user_vouchers");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Voucher", b => {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Code")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                b.Property<DateTime?>("CreatedAt")
                   .HasColumnName("created_at")
                   .HasColumnType("datetime2");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnName("deleted_at")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<decimal?>("DiscountPercent")
                    .HasColumnName("discount_percent")
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTime?>("EndDate")
                    .HasColumnName("end_date")
                    .HasColumnType("datetime2");

                b.Property<bool?>("IsActive")
                    .HasColumnName("is_active")
                    .HasColumnType("bit");

                b.Property<decimal?>("MaxDiscountAmount")
                    .HasColumnName("max_discount_amount")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal?>("MinOrderValue")
                    .HasColumnName("min_order_value")
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTime?>("StartDate")
                    .HasColumnName("start_date")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.ToTable("vouchers");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Cart", b => {
                b.HasOne("SWP391_DEMO.Entities.Customer", "Customer")
                    .WithMany("Carts")
                    .HasForeignKey("CustomerId");

                b.Navigation("Customer");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.CartDetail", b => {
                b.HasOne("SWP391_DEMO.Entities.Cart", "Cart")
                    .WithMany("CartDetails")
                    .HasForeignKey("CartId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SWP391_DEMO.Entities.Product", "Product")
                    .WithMany("CartDetails")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Cart");

                b.Navigation("Product");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Customer", b => {
                b.HasOne("SWP391_DEMO.Entities.User", "User")
                    .WithOne("Customer")
                    .HasForeignKey("SWP391_DEMO.Entities.Customer", "UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.CustomerAddress", b => {
                b.HasOne("SWP391_DEMO.Entities.Customer", "User")
                    .WithMany("CustomerAddresses")
                    .HasForeignKey("UserId");

                b.Navigation("User");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Order", b => {
                b.HasOne("SWP391_DEMO.Entities.Customer", "Customer")
                    .WithMany("Orders")
                    .HasForeignKey("CustomerId");

                b.HasOne("SWP391_DEMO.Entities.OrderStatus", "Status")
                    .WithMany("Orders")
                    .HasForeignKey("StatusId");

                b.HasOne("SWP391_DEMO.Entities.Voucher", "Voucher")
                    .WithMany("Orders")
                    .HasForeignKey("VoucherId");

                b.Navigation("Customer");

                b.Navigation("Status");

                b.Navigation("Voucher");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.OrderDetail", b => {
                b.HasOne("SWP391_DEMO.Entities.Order", "Order")
                    .WithMany("OrderDetails")
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SWP391_DEMO.Entities.Product", "Product")
                    .WithMany("OrderDetails")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Order");

                b.Navigation("Product");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Product", b => {
                b.HasOne("SWP391_DEMO.Entities.Brand", "Brand")
                    .WithMany("Products")
                    .HasForeignKey("BrandId");

                b.HasOne("SWP391_DEMO.Entities.Category", "Category")
                    .WithMany("Products")
                    .HasForeignKey("CategoryId");

                b.HasOne("SWP391_DEMO.Entities.Unit", "Unit")
                    .WithMany("Products")
                    .HasForeignKey("UnitId");

                b.Navigation("Brand");

                b.Navigation("Category");

                b.Navigation("Unit");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAnalytic", b => {
                b.HasOne("SWP391_DEMO.Entities.Product", "Product")
                    .WithMany("ProductAnalytics")
                    .HasForeignKey("ProductId");

                b.Navigation("Product");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAttributeValue", b => {
                b.HasOne("SWP391_DEMO.Entities.ProductAttribute", "Attribute")
                    .WithMany("ProductAttributeValues")
                    .HasForeignKey("AttributeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SWP391_DEMO.Entities.Product", "Product")
                    .WithMany("ProductAttributeValues")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Attribute");

                b.Navigation("Product");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductImage", b => {
                b.HasOne("SWP391_DEMO.Entities.Product", "Product")
                    .WithMany("ProductImages")
                    .HasForeignKey("ProductId");

                b.Navigation("Product");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.RefreshToken", b => {
                b.HasOne("SWP391_DEMO.Entities.User", "User")
                    .WithMany("RefreshTokens")
                    .HasForeignKey("UserId");

                b.Navigation("User");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.User", b => {
                b.HasOne("SWP391_DEMO.Entities.Role", "Role")
                    .WithMany("Users")
                    .HasForeignKey("RoleId");

                b.Navigation("Role");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.UserVoucher", b => {
                b.HasOne("SWP391_DEMO.Entities.Customer", "Customer")
                    .WithMany("UserVouchers")
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SWP391_DEMO.Entities.Voucher", "Voucher")
                    .WithMany("UserVouchers")
                    .HasForeignKey("VoucherId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Customer");

                b.Navigation("Voucher");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Brand", b => {
                b.Navigation("Products");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Cart", b => {
                b.Navigation("CartDetails");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Category", b => {
                b.Navigation("Products");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Customer", b => {
                b.Navigation("Carts");

                b.Navigation("CustomerAddresses");

                b.Navigation("Orders");

                b.Navigation("UserVouchers");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Order", b => {
                b.Navigation("OrderDetails");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.OrderStatus", b => {
                b.Navigation("Orders");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Product", b => {
                b.Navigation("CartDetails");

                b.Navigation("OrderDetails");

                b.Navigation("ProductAnalytics");

                b.Navigation("ProductAttributeValues");

                b.Navigation("ProductImages");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.ProductAttribute", b => {
                b.Navigation("ProductAttributeValues");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Role", b => {
                b.Navigation("Users");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Unit", b => {
                b.Navigation("Products");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.User", b => {
                b.Navigation("Customer");

                b.Navigation("RefreshTokens");
            });

            modelBuilder.Entity("SWP391_DEMO.Entities.Voucher", b => {
                b.Navigation("Orders");

                b.Navigation("UserVouchers");
            });
        }

    }
}
