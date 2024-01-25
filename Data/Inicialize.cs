using Microsoft.AspNetCore.Identity;
using RoomFinder4You.Models;

namespace RoomFinder4You.Data;

public enum Roles{
    Admin,
    Landlord
}

public class Inicialize{

        public static async Task CreateInitialData(UserManager<ApplicationUser>
       userManager, RoleManager<IdentityRole> roleManager)
        {
            //Adicionar default Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Landlord.ToString()));

            //Adicionar Default User - Admin
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Some123!");
                await userManager.AddToRoleAsync(defaultUser,Roles.Admin.ToString());
            }
        }
}