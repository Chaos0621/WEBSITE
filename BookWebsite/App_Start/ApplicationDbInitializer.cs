using BookWebsite.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookWebsite.App_Start
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context); // Call the initialization method
            base.Seed(context);
        }

        private static void InitializeIdentityForEF(ApplicationDbContext context)
        {
            // Create role manager and user manager
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Define admin details
            const string adminEmail = "admin@myStore.com";
            const string adminPassword = "Admin@myStore.com!123";
            const string adminRoleName = "Admin";

            // Check and create the admin role
            if (!roleManager.RoleExists(adminRoleName))
            {
                var roleResult = roleManager.Create(new IdentityRole(adminRoleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Failed to create admin role");
                }
            }

            // Check and create the admin user
            var adminUser = userManager.FindByEmail(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Confirm email by default
                };

                var userResult = userManager.Create(adminUser, adminPassword);
                if (!userResult.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " + string.Join(", ", userResult.Errors));
                }
            }

            // Assign the admin role
            if (!userManager.IsInRole(adminUser.Id, adminRoleName))
            {
                var roleAssignResult = userManager.AddToRole(adminUser.Id, adminRoleName);
                if (!roleAssignResult.Succeeded)
                {
                    throw new Exception("Failed to assign admin role to user: " + string.Join(", ", roleAssignResult.Errors));
                }
            }
        }
    }
}