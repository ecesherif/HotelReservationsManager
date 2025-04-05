using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.LogoutPath = "/Account/Logout"; 
    options.AccessDeniedPath = "/Account/AccessDenied"; 

    options.SlidingExpiration = true; 
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    var roleExistsE = await roleManager.RoleExistsAsync("EMPLOYEE");
    if (!roleExistsE)
    {
        Console.WriteLine("Creating EMPLOYEE role...");
        await roleManager.CreateAsync(new IdentityRole("EMPLOYEE"));
    }
    var roleExistsA = await roleManager.RoleExistsAsync("Admin");
    if (!roleExistsA)
    {
        Console.WriteLine("Creating Admin role...");
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var admin = await userManager.FindByNameAsync("admin");
    if (admin == null)
    {
        Console.WriteLine("Creating admin user...");

        var newAdmin = new User
        {
            UserName = "admin",
            Email = "admin@admin.com",
            EmailConfirmed = true,
            FirstName = "AdminFirstName",
            SecondName = "AdminSecondName",
            LastName = "AdminLastName",
            PhoneNumber = "1234567890",
            EGN = "1234567890",
            HireDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(newAdmin, "Admin@123");
        if (result.Succeeded)
        {
            Console.WriteLine("Admin user created successfully.");
            var roleAddResult = await userManager.AddToRoleAsync(newAdmin, "Admin");
            if (roleAddResult.Succeeded)
            {
                Console.WriteLine("Admin role assigned successfully.");
            }
            else
            {
                foreach (var error in roleAddResult.Errors)
                {
                    Console.WriteLine($"ERROR: {error.Code} - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("Failed to create admin user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"ERROR: {error.Code} - {error.Description}");
            }
        }

    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}
app.Run();
