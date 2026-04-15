using auticare.core;
using auticare.Data;
using auticare.Extentions;
using auticare.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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
var google = builder.Configuration.GetSection("Authentication:Google");



builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
    });
builder.Services.AddScoped<EmailService>();


var app = builder.Build();  // ✅ بعد ما خلصت Services


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