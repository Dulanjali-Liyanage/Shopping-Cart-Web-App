using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartDemo.Areas.Identity.Data;
using ShoppingCartDemo.Data;

[assembly: HostingStartup(typeof(ShoppingCartDemo.Areas.Identity.IdentityHostingStartup))]
namespace ShoppingCartDemo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ShoppingCartDemoContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ShoppingCartDemoContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => 
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                }).AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ShoppingCartDemoContext>();
            });
        }
    }
}