using KartverketWebApp.Services;
using KartverketWebApp;
using KartverketWebApp.API_Models;
using Microsoft.Extensions.Logging; 

var builder = WebApplication.CreateBuilder(args);

// Bind the API settings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register services and their interfaces
builder.Services.AddHttpClient<IStednavn, StednavnService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
