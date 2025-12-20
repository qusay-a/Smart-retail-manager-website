using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connString));


builder.Services.AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


builder.Services.AddDistributedMemoryCache();
// Session
builder.Services.AddSession();


// Simple cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<BillRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("RemoteClientPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:5002", "http://localhost:5002") // <-- remote app URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("RemoteClientPolicy");
app.MapControllers();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
