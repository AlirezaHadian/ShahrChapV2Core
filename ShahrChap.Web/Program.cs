using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Senders;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Product;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options=> options.EnableEndpointRouting = false);
builder.Services.AddRazorPages();
#region Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options=>
{
    options.LoginPath= "/Login";
    options.LogoutPath = "/Logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);
});
builder.Services.AddAuthorization();
#endregion
#region Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion
#region DataBase Context
var dbConnectionString = builder.Configuration.GetSection("ConnectionStrings:ShahrChapDatabase").Value;

builder.Services.AddDbContext<ShahrChapContext>(options=>
{
    options.UseSqlServer(dbConnectionString);
});
#endregion
#region IoC
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddScoped<MessageSender>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IViewRenderService, RenderViewToString>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<IProductService, ProductService>();
#endregion

var app = builder.Build();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages(); 

// app.UseMvc(routes =>
// {
//     routes.MapRoute(name: "Area", template: "{areas:exists}/{controller=Home}/{action=Index}/{id?}");
//     routes.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
// });

app.MapControllerRoute(
    name: "Area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapGet("/", () => "Hello World!");

app.Run();
