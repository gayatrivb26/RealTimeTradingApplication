

using Microsoft.AspNetCore.Identity;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Infrastructure.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            var email = "admin@tradingapp.com";

            var admin = await userManager.FindByEmailAsync(email);
            if (admin != null) return;

            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "System",
                LastName = "Admin",
            };

            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
