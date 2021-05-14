using Microsoft.AspNetCore.Identity;
using ShoppingCartDemo.Areas.Identity.Data;

namespace ShoppingCartDemo.Models
{
    public class UserAndRoleDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("dulanjali@gmail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "dulanjali@gmail.com";
                user.Email = "dulanjali@gmail.com";
                user.FirstName = "Dulanjali";
                user.LastName = "Liyanage";

                IdentityResult result = userManager.CreateAsync(user, "abc123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "CartUser").Wait();
                }
            }


            if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "admin@gmail.com";
                user.Email = "admin@gmail.com";
                user.FirstName = "Kalana";
                user.LastName = "Liyanage";

                IdentityResult result = userManager.CreateAsync(user, "abc123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }


        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("CartUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "CartUser";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
