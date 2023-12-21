using MehranBot.Models.Context;
using MehranBot.Models.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//builder.Services.AddDbContext<MehranBotDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));

builder.Services.AddDbContext<MehranBotDbContext>(
        options => options.UseSqlServer(
            "Your Connection String",
            providerOptions => providerOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
