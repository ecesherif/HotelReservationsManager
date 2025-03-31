using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register the ApplicationDbContext with the correct connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add identity services and configure UserManager with your custom User class
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Register roles
    .AddEntityFrameworkStores<ApplicationDbContext>() // Use your custom ApplicationDbContext
    .AddDefaultTokenProviders(); // Add default token providers for things like password reset

// Add services for Razor Pages and Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline for Development and Production environments
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map routes for Controllers and Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();  // Maps Razor Pages endpoints

// Add role and user seeding logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Seed roles if they do not exist
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                var identityRole = new IdentityRole(role);
                var result = await roleManager.CreateAsync(identityRole);
                if (!result.Succeeded)
                {
                    // Log error if role creation fails
                    Console.WriteLine($"Error creating role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Assign "Admin" role to users with email ending in "@admin.com"
        var adminUsers = await userManager.Users.Where(u => u.Email.EndsWith("@admin.com")).ToListAsync();
        foreach (var user in adminUsers)
        {
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding roles and users: {ex.Message}");
    }
}

app.Run();