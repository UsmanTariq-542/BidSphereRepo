using BidSphereProject.Data;
using BidSphereProject.Hubs;
using BidSphereProject.Interfaces;
using BidSphereProject.Repositories;
using BidSphereProject.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BidSphereProject
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    googleOptions.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always; 
                    googleOptions.CorrelationCookie.SameSite = SameSiteMode.Lax; 
                });

            builder.Services.ConfigureApplicationCookie(options => {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // Dependency injection 
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
            builder.Services.AddScoped<IBidRepository, BidRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();  

            builder.Services.AddScoped<AuctionService>(); // for auction creation in auctionServices class


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => 
                policy.RequireClaim("IsAdmin", "true"));   // for protecting admin page and functionalities.
            
            });

            var app = builder.Build();

            // for submission purposes, it automatically generates required identity tables etc 
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated(); // <- here
            }

            // removed for testing purposes, should be added in production
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


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapHub<NotificationHub>("/notificationHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }

    }

    
}
