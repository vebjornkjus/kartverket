using KartverketWebApp.Services;
using KartverketWebApp;
using KartverketWebApp.API_Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using KartverketWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using System.Data;
using MySqlConnector;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


// Bind the API settings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register services and their interfaces
builder.Services.AddHttpClient<ISokeService, SokeService>();
builder.Services.AddHttpClient<IStednavn, StednavnService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Aktiver CSRF-beskyttelse globalt
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()); 
});

// Add PasswordHasher
builder.Services.AddScoped<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        new MySqlServerVersion(new Version(10, 5, 9)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

// Add this line to register IDbConnection
builder.Services.AddTransient<IDbConnection>(sp =>
    new MySqlConnection(builder.Configuration.GetConnectionString("MariaDbConnection")));


// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cookies for Autentisering
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "KartverketAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Home/Index"; // Omdiriger til Index hvis tilgang nektes
    });

// Use AddAuthorizationBuilder for policies
var authorizationBuilder = builder.Services.AddAuthorizationBuilder();
authorizationBuilder.AddPolicy("AdminOrSaksbehandlerPolicy", policy =>
    policy.RequireClaim("BrukerType", "admin", "saksbehandler")); // Sjekker om BrukerType er admin eller saksbehandler

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

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
