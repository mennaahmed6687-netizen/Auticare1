using System.Text.Json.Serialization;
using auticare.core;
using auticare.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuticareDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("auticareContext") ?? throw new InvalidOperationException("Connection string 'AuticareDbContext' not found.")));
builder.Services.AddSingleton<IdataChild<Child>, UserEntity >();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc(op => op.EnableEndpointRouting = false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseMvcWithDefaultRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSwagger();
app.UseSwaggerUI();
app.Run();

