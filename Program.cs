// In Program.cs (.NET 6+)
using Microsoft.EntityFrameworkCore;
using MovieDB.Data; // Adjust to your DbContext namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // This registers MVC services

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(connectionString)); // Or your chosen provider

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Standard MVC error handling
    app.UseHsts();
} else
{
    // Optional: Seed database in development (similar to Blazor)
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    // dbContext.Database.Migrate(); // Apply migrations
    // DataSeeder.Initialize(dbContext); // Your seeding logic
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // For wwwroot content (CSS, JS, images)

app.UseRouting();

app.UseAuthorization(); // If you add authentication/authorization

// Default MVC route: {controller=Home}/{action=Index}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();