using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyEshop.Data;
using MyEshop.Data.Repositories;
using YourNamespace.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region Db Context
builder.Services.AddDbContext<MyEshopContext>(options =>
{
    options.UseSqlServer("Data Source=.;Initial Catalog=EshopCore_DB;Integrated Security=true;TrustServerCertificate=True;");
});
#endregion

#region Ioc
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

#region Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login";
        option.LogoutPath = "/Account/Logout"; // ????? ???
        option.ExpireTimeSpan = TimeSpan.FromDays(10);
    });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseAdminAccessMiddleware();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

