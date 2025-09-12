using Microsoft.AspNetCore.Identity;

namespace Uber.Uber.Domain.Entities
{
    public class Roles
    {
        public static async Task SeedRolesAsync(RoleManager<RoleApp> roleManager)
        {
            string[] roles = { "Admin", "Merchant", "Driver", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new RoleApp { Name = role });
                }
            }
        }
        List<User> users { get; set; } = new List<User>();

    }
}
