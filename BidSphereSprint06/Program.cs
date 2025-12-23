using BidSphereProject.Data;
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

            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "329991947819-h9av0jc720jvac6s92ic2aouddprtpvs.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "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";
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
            

                options.AddPolicy("ContactOpen", policy =>
                    policy.RequireAssertion(context =>
                        DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 18      // like in just business hours only website accessed
                ));
            });

            var app = builder.Build();

            // for submission purposes, it automatically generates required identity tables etc 
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated(); // <- here
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
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
