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
builder.Logging.AddConsole();

// Set default culture
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

// Enable CSRF protection globally
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

// Add IDbConnection for raw database queries
builder.Services.AddTransient<IDbConnection>(sp =>
    new MySqlConnection(builder.Configuration.GetConnectionString("MariaDbConnection")));

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cookies for authentication
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "KartverketAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true; // Protection against XSS
    });

// Use AddAuthorizationBuilder for policies
var authorizationBuilder = builder.Services.AddAuthorizationBuilder();
authorizationBuilder.AddPolicy("AdminOrSaksbehandlerPolicy", policy =>
    policy.RequireClaim("BrukerType", "admin", "saksbehandler")); // Check if BrukerType is admin or saksbehandler
authorizationBuilder.AddPolicy("AdminPolicy", policy =>
    policy.RequireClaim("BrukerType", "admin"));

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

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Default route
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Meldinger-specific route
    endpoints.MapControllerRoute(
        name: "meldinger",
        pattern: "meldinger/{action=Index}/{rapportId?}",
        defaults: new { controller = "Meldinger" });

    // Saksbehandler-specific route
    endpoints.MapControllerRoute(
        name: "saksbehandler",
        pattern: "saksbehandler/{action=Index}/{rapportId?}",
        defaults: new { controller = "Saksbehandler" });

    // Dette mønsteret vil matche Detaljert controller actions
    endpoints.MapControllerRoute(
        name: "detaljert",
        pattern: "Detaljert/{action}/{id?}",
        defaults: new { controller = "Detaljert" });

});

app.Run();
