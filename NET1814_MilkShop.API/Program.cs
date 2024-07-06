using Serilog;

namespace NET1814_MilkShop.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Write log
        _ = builder.Host.UseSerilog(
            (hostContext, loggerConfiguration) =>
                _ = loggerConfiguration.ReadFrom.Configuration(builder.Configuration)
        );
        // Add services to the container.
        var startup = new Startup(builder, builder.Environment);
        startup.ConfigureServices(builder.Services);
        // Build the container.
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        startup.Configure(app, builder.Environment);
        // Run the application.
        app.Run();
    }
}