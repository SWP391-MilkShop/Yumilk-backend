using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.Infrastructure;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Models.MailModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services;
using System.Text;

namespace NET1814_MilkShop.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            _configuration = builder.Configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "NET1814_MilkShop.API", Version = "v1" });
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false;
            });
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                throw new InvalidOperationException(
                    "Could not find connection string 'DefaultConnection'"
                );
            }
            //Add Dependency Injection
            AddDI(services);
            //Add Email Setting
            services.Configure<EmailSettingModel>(_configuration.GetSection("EmailSettings")); //fix EmailSetting thanh EmailSettings ngồi mò gần 2 tiếng :D
            //Add Database
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            //Add Exception Handler
            services.AddExceptionHandler<ExceptionLoggingHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            //Add Cors
            services.AddCors(services =>
            {
                services.AddPolicy(
                    "DefaultPolicy",
                    builder =>
                    {
                        //cho nay de domain web cua minh
                        builder
                            .WithOrigins("https://localhost:5000", "http://localhost:5001") // Allow only these origins
                            .WithMethods("GET", "POST", "PUT", "DELETE") // Allow only these methods
                            .AllowAnyHeader();
                    }
                );
                services.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });
            //Add Authentication
            services.AddAuthentication().AddJwtBearer("Access", o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:AccessTokenKey"]))
                };
            })
            .AddJwtBearer("Refresh", o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshTokenKey"]))
                };
            });

        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var isUserSwagger = _configuration.GetValue<bool>("UseSwagger", false);
            if (isUserSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DefaultModelsExpandDepth(-1);
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NET1814_MilkShop.API v1");
                });
            }

            app.UseRouting();
            app.UseCors("AllowAll"); //luon dat truoc app.UseAuthorization()
            app.UseAuthorization();
            app.UseExceptionHandler(options => { });
            // ko biet sao cai nay no keu violate ASP0014, keu map route truc tiep trong api luon
            //app.UseEndpoints(endpoint =>
            //{
            //    endpoint.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.MapControllers();
        }

        private static void AddDI(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailService, EmailService>();
            //Add Extensions
            services.AddScoped<IJwtTokenExtension, JwtTokenExtension>();
            //Add Filters
            services.AddScoped<UserExistsFilter>();
        }
    }
}
