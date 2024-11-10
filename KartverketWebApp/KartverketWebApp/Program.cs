using KartverketWebApp.Services;
using KartverketWebApp;
using KartverketWebApp.API_Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using KartverketWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;

var builder = WebApplication.CreateBuilder(args);

// Bind the API settings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register services and their interfaces
builder.Services.AddHttpClient<ISokeService, SokeService>();
builder.Services.AddHttpClient<IStednavn, StednavnService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add PasswordHasher
builder.Services.AddScoped<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDbConnection"),
    new MySqlServerVersion(new Version(10, 5, 9))));    // replace with your MariaDB version

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add logging configuration
builder.Logging.ClearProviders();  // Clear any default logging providers
builder.Logging.AddConsole();      // Add console logging so logs show up in the terminal
builder.Logging.SetMinimumLevel(LogLevel.Information);  // Set minimum log level to Information

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

app.UseAuthentication();  // Enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
