using ECommerceApp.Data;
using ECommerceApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ECommerceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // This will use the property names as defined in the C# model
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add Swagger (this enables the Swagger UI)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "API for managing customers, orders, etc."
                });
            });

            // Configure EF Core with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));

            // Registering the CustomerService
            builder.Services.AddScoped<CustomerService>();

            // Registering the AddressService
            builder.Services.AddScoped<AddressService>();

            // Registering the CategoryService
            builder.Services.AddScoped<CategoryService>();

            // Registering the ProductService
            builder.Services.AddScoped<ProductService>();

            // Registering the ShoppingCartService
            builder.Services.AddScoped<ShoppingCartService>();

            // Registering the OrderService
            builder.Services.AddScoped<OrderService>();

            // Registering the PaymentService
            builder.Services.AddScoped<PaymentService>();

            // Registering the EmailService 
            builder.Services.AddScoped<EmailService>();

            // Registering the CancellationService
            builder.Services.AddScoped<CancellationService>();

            // Registering the RefundService
            builder.Services.AddScoped<RefundService>();    

            // Registering the FeedbackService
            builder.Services.AddScoped<FeedbackService>();

            // Register Refund Processing Background Service
            builder.Services.AddHostedService<RefundProcessingBackgroundService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger and Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API V1");
                    c.RoutePrefix = string.Empty; // Swagger UI will open at root ("/")
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
     

        }
    }
}
