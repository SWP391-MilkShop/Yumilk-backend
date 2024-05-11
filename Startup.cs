using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SWP391_DEMO.Data;
using SWP391_DEMO.Repository;
using SWP391_DEMO.Service;

namespace SWP391_DEMO
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
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "SWP391_DEMO", Version = "v1" });
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
                throw new InvalidOperationException("Could not find connection string 'DefaultConnection'");
            }
            AddDI(services);
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            //services.AddAuthentication("Bearer").AddJwtBearer(o =>
            //{
            //    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            //    };
            //});
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SWP391_DEMO v1");
                });
            }
            app.MapControllers();
            app.UseRouting();
            app.UseAuthorization();
            // ko biet sao cai nay no keu violate ASP0014, keu map route truc tiep trong api luon
            //app.UseEndpoints(endpoint =>
            //{
            //    endpoint.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
        }

        private void AddDI(IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
        }
    }
}

