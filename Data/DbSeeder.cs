using Microsoft.AspNetCore.Identity;
using FoodDonationPlatform.Models;

namespace FoodDonationPlatform.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            string[] roles = { "Admin", "LocalUser" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            var adminEmail = "admin@fooddonation.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Platform Admin",
                    Address = "18, Sector 7, Kharghar, Navi Mumbai, Maharashtra - 410210",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Local User
            var userEmail = "user@fooddonation.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FullName = "Rajesh Misra",
                    Address = "42, Sector 15, Rohini, New Delhi - 110085",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "User@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "LocalUser");
            }

            // Seed Food Listings if empty
            if (!context.FoodListings.Any())
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                var localUser = await userManager.FindByEmailAsync(userEmail);

                if (adminUser != null && localUser != null)
                {
                    var listings = new List<FoodListing>
                    {
                        new() { FoodName = "Fresh Vegetable Soup", Description = "Homemade vegetable soup with carrots, celery, and potatoes. Made fresh this morning.", Quantity = 20, Unit = "servings", ExpirationDate = DateTime.UtcNow.AddDays(2), Location = "B-27, Sector 62, Noida, Uttar Pradesh - 201309", Category = "Hot Meals", DonorId = adminUser.Id },
                        new() { FoodName = "Whole Wheat Bread", Description = "Freshly baked whole wheat loaves. Great for sandwiches or toast.", Quantity = 15, Unit = "loaves", ExpirationDate = DateTime.UtcNow.AddDays(3), Location = "22/4, Park Street, Kolkata, West Bengal - 700016", Category = "Baked Goods", DonorId = adminUser.Id },
                        new() { FoodName = "Canned Beans", Description = "Mixed variety of canned beans including black beans, Rajma, and chole.", Quantity = 50, Unit = "cans", ExpirationDate = DateTime.UtcNow.AddDays(365), Location = "114, MG Road, Indiranagar, Bengaluru - 560038", Category = "Pantry Items", DonorId = adminUser.Id },
                        new() { FoodName = "Brown Rice (Bulk)", Description = "Premium long-grain brown rice. High in fiber and nutrients.", Quantity = 100, Unit = "lbs", ExpirationDate = DateTime.UtcNow.AddDays(180), Location = "75, Navrangpura, Ahmedabad, Gujarat - 380009", Category = "Grains", DonorId = adminUser.Id },
                        new() { FoodName = "Fresh Fruit Basket", Description = "Assorted seasonal fruits including apples, oranges, and bananas. Perfect for snacking.", Quantity = 30, Unit = "pieces", ExpirationDate = DateTime.UtcNow.AddDays(5), Location = "56, Sector 21, Dwarka, New Delhi - 110075", Category = "Fresh Produce", DonorId = localUser.Id },
                        new() { FoodName = "Pasta & Sauce Kit", Description = "Complete meal kit with spaghetti and marinara sauce. Easy to prepare.", Quantity = 25, Unit = "kits", ExpirationDate = DateTime.UtcNow.AddDays(90), Location = "6A/Bismillah street, Railway Colony, Surat - 75300", Category = "Pantry Items", DonorId = localUser.Id },
                    };

                    context.FoodListings.AddRange(listings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
