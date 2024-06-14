using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext() { }

        public AppDbContext(DbContextOptions options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartDetail> CartDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<ProductAnalytic> ProductAnalytics { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductStatus> ProductStatuses { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<UserVoucher> UserVouchers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ////////////////////////////////////////////////////////////////
            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Brand", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<string>("Logo")
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("logo");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AS");

                b.HasKey("Id");

                b.HasIndex("Name");

                b.ToTable("brands", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Cart", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<Guid>("CustomerId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("customer_id");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.HasKey("Id");

                b.HasIndex("CustomerId")
                    .IsUnique();

                b.ToTable("carts", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.CartDetail", b =>
            {
                b.Property<int>("CartId")
                    .HasColumnType("int")
                    .HasColumnName("cart_id");

                b.Property<Guid>("ProductId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("product_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<int>("Quantity")
                    .HasColumnType("int");

                b.HasKey("CartId", "ProductId");

                b.HasIndex("ProductId");

                b.ToTable("cart_details", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Category", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AI");

                b.HasKey("Id");

                b.HasIndex("Name");

                b.ToTable("categories", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Customer", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("user_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Email")
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("email");

                b.Property<string>("GoogleId")
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("google_id");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("nvarchar(20)")
                    .HasColumnName("phone_number");

                b.Property<int>("Points")
                    .HasColumnType("int")
                    .HasColumnName("points");

                b.Property<string>("ProfilePictureUrl")
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("profile_picture_url");

                b.HasKey("UserId");

                b.HasIndex("Email")
                    .IsUnique()
                    .HasFilter("[email] IS NOT NULL");

                b.HasIndex("GoogleId")
                    .IsUnique()
                    .HasFilter("[google_id] IS NOT NULL");

                b.HasIndex("PhoneNumber")
                    .IsUnique()
                    .HasFilter("[phone_number] IS NOT NULL");

                b.ToTable("customers", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.CustomerAddress", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Address")
                    .HasColumnType("nvarchar(2000)")
                    .HasColumnName("address");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<int>("DistrictId")
                    .HasColumnType("int")
                    .HasColumnName("district_id");

                b.Property<string>("DistrictName")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("district_name");

                b.Property<bool>("IsDefault")
                    .HasColumnType("bit")
                    .HasColumnName("is_default");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("nvarchar(20)")
                    .HasColumnName("phone_number");

                b.Property<int>("ProvinceId")
                    .HasColumnType("int")
                    .HasColumnName("province_id");

                b.Property<string>("ProvinceName")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("province_name");

                b.Property<string>("ReceiverName")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("receiver_name");

                b.Property<Guid?>("UserId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("user_id");

                b.Property<string>("WardCode")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("ward_code");

                b.Property<string>("WardName")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("ward_name");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("customer_addresses", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Order", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Address")
                    .IsRequired()
                    .HasColumnType("nvarchar(2000)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<Guid?>("CustomerId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("customer_id");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Note")
                    .HasColumnType("nvarchar(max)");

                b.Property<int?>("OrderCode")
                    .HasColumnType("int")
                    .HasColumnName("order_code");

                b.Property<DateTime?>("PaymentDate")
                    .HasColumnType("datetime2")
                    .HasColumnName("payment_date");

                b.Property<string>("PaymentMethod")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("payment_method");

                b.Property<string>("PhoneNumber")
                    .IsRequired()
                    .HasColumnType("nvarchar(20)")
                    .HasColumnName("phone_number");

                b.Property<decimal>("ShippingFee")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("shipping_fee");

                b.Property<int>("StatusId")
                    .HasColumnType("int")
                    .HasColumnName("status_id");

                b.Property<decimal>("TotalAmount")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("total_amount");

                b.Property<decimal>("TotalPrice")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("total_price");

                b.Property<int>("VoucherId")
                    .HasColumnType("int")
                    .HasColumnName("voucher_id");

                b.HasKey("Id");

                b.HasIndex("CustomerId");

                b.HasIndex("StatusId");

                b.HasIndex("VoucherId");

                b.ToTable("orders", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.OrderDetail", b =>
            {
                b.Property<Guid>("OrderId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("order_id");

                b.Property<Guid>("ProductId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("product_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<decimal>("ItemPrice")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("item_price");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("ProductName")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("product_name");

                b.Property<int>("Quantity")
                    .HasColumnType("int");

                b.Property<decimal>("UnitPrice")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("unit_price");

                b.HasKey("OrderId", "ProductId");

                b.HasIndex("ProductId");

                b.ToTable("order_details", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.OrderStatus", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AS");

                b.HasKey("Id");

                b.ToTable("order_statuses", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Product", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("BrandId")
                    .HasColumnType("int")
                    .HasColumnName("brand_id");

                b.Property<int>("CategoryId")
                    .HasColumnType("int")
                    .HasColumnName("category_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(MAX)")
                    .UseCollation("Latin1_General_CI_AI");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AI");

                b.Property<decimal>("OriginalPrice")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("decimal(18,3)")
                    .HasDefaultValue(0m)
                    .HasColumnName("original_price");

                b.Property<int>("Quantity")
                    .HasColumnType("int");

                b.Property<decimal>("SalePrice")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("decimal(18,3)")
                    .HasDefaultValue(0m)
                    .HasColumnName("sale_price");

                b.Property<int>("StatusId")
                    .HasColumnType("int")
                    .HasColumnName("status_id");

                b.Property<string>("Thumbnail")
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("thumbnail");

                b.Property<int>("UnitId")
                    .HasColumnType("int")
                    .HasColumnName("unit_id");

                b.HasKey("Id");

                b.HasIndex("BrandId");

                b.HasIndex("CategoryId");

                b.HasIndex("StatusId");

                b.HasIndex("UnitId");

                b.ToTable("products", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAnalytic", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<Guid?>("ProductId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("product_id");

                b.Property<int>("PurchaseCount")
                    .HasColumnType("int")
                    .HasColumnName("purchase_count");

                b.Property<int>("ViewCount")
                    .HasColumnType("int")
                    .HasColumnName("view_count");

                b.HasKey("Id");

                b.HasIndex("ProductId");

                b.ToTable("product_analytics", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAttribute", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AS");

                b.HasKey("Id");

                b.ToTable("product_attributes", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAttributeValue", b =>
            {
                b.Property<Guid>("ProductId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("product_id");

                b.Property<int>("AttributeId")
                    .HasColumnType("int")
                    .HasColumnName("attribute_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar(2000)");

                b.HasKey("ProductId", "AttributeId");

                b.HasIndex("AttributeId");

                b.ToTable("product_attribute_values", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductImage", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("ImageUrl")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .HasColumnName("image_url");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<Guid?>("ProductId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("product_id");

                b.HasKey("Id");

                b.HasIndex("ProductId");

                b.ToTable("product_images", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductStatus", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("product_statuses");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Role", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.HasKey("Id");

                b.ToTable("roles", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Unit", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)")
                    .UseCollation("Latin1_General_CI_AS");

                b.HasKey("Id");

                b.HasIndex("Name");

                b.ToTable("units", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("FirstName")
                    .HasColumnType("nvarchar(50)")
                    .HasColumnName("first_name");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<bool>("IsBanned")
                    .HasColumnType("bit")
                    .HasColumnName("is_banned");

                b.Property<string>("LastName")
                    .HasColumnType("nvarchar(50)")
                    .HasColumnName("last_name");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<string>("Password")
                    .IsRequired()
                    .HasColumnType("nvarchar(255)");

                b.Property<string>("ResetPasswordCode")
                    .HasColumnType("nvarchar(6)")
                    .HasColumnName("reset_password_code");

                b.Property<int>("RoleId")
                    .HasColumnType("int")
                    .HasColumnName("role_id");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)")
                    .UseCollation("Latin1_General_CS_AS");

                b.Property<string>("VerificationCode")
                    .HasColumnType("nvarchar(6)")
                    .HasColumnName("verification_code");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("users", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.UserVoucher", b =>
            {
                b.Property<Guid>("CustomerId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("customer_id");

                b.Property<int>("VoucherId")
                    .HasColumnType("int")
                    .HasColumnName("voucher_id");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.HasKey("CustomerId", "VoucherId");

                b.HasIndex("VoucherId");

                b.ToTable("user_vouchers", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Voucher", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Code")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("deleted_at");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(2000)");

                b.Property<decimal>("DiscountPercent")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("discount_percent");

                b.Property<DateTime?>("EndDate")
                    .HasColumnType("datetime2")
                    .HasColumnName("end_date");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit")
                    .HasColumnName("is_active");

                b.Property<decimal>("MaxDiscountAmount")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("max_discount_amount");

                b.Property<decimal>("MinOrderValue")
                    .HasColumnType("decimal(18,3)")
                    .HasColumnName("min_order_value");

                b.Property<DateTime?>("ModifiedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("modified_at");

                b.Property<DateTime?>("StartDate")
                    .HasColumnType("datetime2")
                    .HasColumnName("start_date");

                b.HasKey("Id");

                b.ToTable("vouchers", null as string);
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Cart", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Customer", "Customer")
                    .WithMany("Carts")
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Customer");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.CartDetail", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Cart", "Cart")
                    .WithMany("CartDetails")
                    .HasForeignKey("CartId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Product", "Product")
                    .WithMany("CartDetails")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Cart");

                b.Navigation("Product");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Customer", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.User", "User")
                    .WithOne("Customer")
                    .HasForeignKey("NET1814_MilkShop.Repositories.Data.Entities.Customer", "UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.CustomerAddress", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Customer", "User")
                    .WithMany("CustomerAddresses")
                    .HasForeignKey("UserId");

                b.Navigation("User");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Order", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Customer", "Customer")
                    .WithMany("Orders")
                    .HasForeignKey("CustomerId");

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.OrderStatus", "Status")
                    .WithMany("Orders")
                    .HasForeignKey("StatusId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Voucher", "Voucher")
                    .WithMany("Orders")
                    .HasForeignKey("VoucherId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Customer");

                b.Navigation("Status");

                b.Navigation("Voucher");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.OrderDetail", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Order", "Order")
                    .WithMany("OrderDetails")
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Product", "Product")
                    .WithMany("OrderDetails")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Order");

                b.Navigation("Product");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Product", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Brand", "Brand")
                    .WithMany("Products")
                    .HasForeignKey("BrandId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Category", "Category")
                    .WithMany("Products")
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.ProductStatus", "ProductStatus")
                    .WithMany()
                    .HasForeignKey("StatusId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Unit", "Unit")
                    .WithMany("Products")
                    .HasForeignKey("UnitId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Brand");

                b.Navigation("Category");

                b.Navigation("ProductStatus");

                b.Navigation("Unit");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAnalytic", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Product", "Product")
                    .WithMany("ProductAnalytics")
                    .HasForeignKey("ProductId");

                b.Navigation("Product");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAttributeValue", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.ProductAttribute", "Attribute")
                    .WithMany("ProductAttributeValues")
                    .HasForeignKey("AttributeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Product", "Product")
                    .WithMany("ProductAttributeValues")
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Attribute");

                b.Navigation("Product");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductImage", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Product", "Product")
                    .WithMany("ProductImages")
                    .HasForeignKey("ProductId");

                b.Navigation("Product");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.User", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Role", "Role")
                    .WithMany("Users")
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Role");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.UserVoucher", b =>
            {
                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Customer", "Customer")
                    .WithMany("UserVouchers")
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("NET1814_MilkShop.Repositories.Data.Entities.Voucher", "Voucher")
                    .WithMany("UserVouchers")
                    .HasForeignKey("VoucherId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Customer");

                b.Navigation("Voucher");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Brand", b =>
            {
                b.Navigation("Products");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Cart", b =>
            {
                b.Navigation("CartDetails");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Category", b =>
            {
                b.Navigation("Products");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Customer", b =>
            {
                b.Navigation("Carts");

                b.Navigation("CustomerAddresses");

                b.Navigation("Orders");

                b.Navigation("UserVouchers");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Order", b =>
            {
                b.Navigation("OrderDetails");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.OrderStatus", b =>
            {
                b.Navigation("Orders");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Product", b =>
            {
                b.Navigation("CartDetails");

                b.Navigation("OrderDetails");

                b.Navigation("ProductAnalytics");

                b.Navigation("ProductAttributeValues");

                b.Navigation("ProductImages");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.ProductAttribute", b =>
            {
                b.Navigation("ProductAttributeValues");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Role", b =>
            {
                b.Navigation("Users");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Unit", b =>
            {
                b.Navigation("Products");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.User", b =>
            {
                b.Navigation("Customer");
            });

            modelBuilder.Entity("NET1814_MilkShop.Repositories.Data.Entities.Voucher", b =>
            {
                b.Navigation("Orders");

                b.Navigation("UserVouchers");
            });
        }
    }
}