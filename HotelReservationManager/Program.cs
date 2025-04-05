using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;

namespace HotelReservationManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                        // Ensure appsettings.json is loaded (this is default behavior)
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;

                        // Ensure the connection string is correctly passed
                        var connectionString = configuration.GetConnectionString("DefaultConnection");
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                        }

                        // Set up DbContext with connection string
                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(connectionString));

                        services.AddIdentity<User, IdentityRole>(options => { })
                            .AddEntityFrameworkStores<ApplicationDbContext>();

                        services.ConfigureApplicationCookie(opts =>
                        {
                            opts.LoginPath = "/Identity/Account/Login";
                            opts.AccessDeniedPath = "/Identity/Account/AccessDenied";
                        });

                        services.AddControllersWithViews();
                        services.AddRazorPages();
                    })
                    .Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                            app.UseDatabaseErrorPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();

                        app.UseRouting();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseStatusCodePagesWithRedirects("/Home/StatusCode?code={0}");

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapRazorPages();
                        });
                    });
                });
    }
}
