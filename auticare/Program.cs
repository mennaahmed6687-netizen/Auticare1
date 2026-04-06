using auticare.core;
using auticare.Data;
using auticare.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==========================
        // 1️⃣ قاعدة البيانات
        // ==========================
        builder.Services.AddDbContext<AuticareDbContext>(options =>
            options.UseLazyLoadingProxies()
                   .UseSqlServer(builder.Configuration.GetConnectionString("auticareContext")
                       ?? throw new InvalidOperationException("Connection string not found."))
        );

        // ==========================
        // 2️⃣ Identity
        // ==========================
        builder.Services.AddIdentity<Parent, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;

            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuticareDbContext>()
        .AddDefaultTokenProviders();

        // ==========================
        // 3️⃣ JWT (🔥 مهم جدًا)
        // ==========================
        builder.Services.AddCustomJwtAuth(builder.Configuration);

        // ==========================
        // 4️⃣ Services
        // ==========================
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });

     

        builder.Services.AddSingleton<IdataChild<Child>, UserEntity>();

        builder.Services.AddControllers()
            .AddNewtonsoftJson()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // ==========================
        // 5️⃣ Swagger + JWT
        // ==========================
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenJwtAuth();

        // ==========================
        // Build
        // ==========================
        var app = builder.Build();

        // ==========================
        // Middleware
        // ==========================
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        // ==========================
        // Routes
        // ==========================
        app.UseCors("AllowAll");
        app.MapControllers();

        app.Run();
        
    }
}