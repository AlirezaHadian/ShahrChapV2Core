using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Options;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.Web.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllersWithViews(options=> options.EnableEndpointRouting = false);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
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
builder.Services.AddScoped<IOtpStorage, SessionOtpStorage>();
builder.Services.Configure<MessageSenderOptions>(builder.Configuration.GetSection("MessageSender"));
builder.Services.AddScoped<ISMSService, SMSService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IOtpService, OtpService>();
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
