using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCartDemo.Areas.Identity.Data;
using ShoppingCartDemo.Models;

namespace ShoppingCartDemo.Data
{
    public class ShoppingCartDemoContext : IdentityDbContext<ApplicationUser>
    {
        
        public ShoppingCartDemoContext(DbContextOptions<ShoppingCartDemoContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        
    }
}
