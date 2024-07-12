using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options=> options.EnableEndpointRouting = false);

#region DataBase Context
var dbConnectionString = builder.Configuration.GetSection("ConnectionStrings:ShahrChapDatabase").Value;

builder.Services.AddDbContext<ShahrChapContext>(options=>
{
    options.UseSqlServer(dbConnectionString);
});
#endregion

#region IoC
builder.Services.AddTransient<IUserService, UserService>();
#endregion

var app = builder.Build();
app.UseMvcWithDefaultRoute();
app.UseStaticFiles();

app.MapGet("/", () => "Hello World!");

app.Run();
