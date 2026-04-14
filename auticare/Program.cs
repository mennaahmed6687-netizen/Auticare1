using auticare.core;
using auticare.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using auticare.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// DbContext
// ==========================
builder.Services.AddDbContext<AuticareDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("auticareContext")
               ?? throw new InvalidOperationException("Connection string not found."))
);

// ==========================
// Identity
// ==========================
builder.Services.AddIdentity<Parent, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    options.User.AllowedUserNameCharacters =
   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuticareDbContext>()
.AddDefaultTokenProviders();

// ==========================
// JWT
// ==========================
builder.Services.AddCustomJwtAuth(builder.Configuration);

// ==========================
// CORS
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        policy => policy.WithOrigins("http://127.0.0.1:5500")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ==========================
// Controllers
// ==========================
builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ==========================
// Swagger
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
app.UseStaticFiles();
app.UseRouting();

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();