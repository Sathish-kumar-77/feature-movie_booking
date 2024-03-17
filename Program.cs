using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieBooking.Data;
using MovieBooking.Data.Cart;
using MovieBooking.Data.Services;
using MovieBooking.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(option=>{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Configure services
builder.Services.AddScoped<IActorsService,ActorService>();
builder.Services.AddScoped<IProducersService,ProducersService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IMoviesService,MovieService>();
builder.Services.AddScoped<IOrderService ,OrderService>();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddScoped(sc=>ShoppingCart.GetShoppingCart(sc) );

builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(option =>{
    option.DefaultScheme =CookieAuthenticationDefaults.AuthenticationScheme;
});

builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//seed Database

AddDbInitializer.Seed(app);   
AddDbInitializer.SeedUserAndRoleAsync(app).Wait();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

 

app.Run();
