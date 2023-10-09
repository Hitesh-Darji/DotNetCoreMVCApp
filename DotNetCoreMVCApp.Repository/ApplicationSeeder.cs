using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreMVCApp.Models.Repository;

namespace DotNetCoreMVCApp.Repository
{
    public class ApplicationSeeder
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILog _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationSeeder(ApplicationDbContext ctx,
           RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _logger = LogManager.GetLogger(typeof(ApplicationSeeder));
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            try
            {
                _logger.Info("Ensuring database created");
                _ctx.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger?.Error("Failed to ensure database is created");
                _logger?.Error(ex.Message);
                throw;
            }

            _logger.Info("Seeding data started");

            _logger.Info("Seeding SuperAdmin role");
            var superAdminRoleName = "Super Admin";
            var adminRole = await _roleManager.FindByNameAsync(superAdminRoleName);
            if (adminRole == null)
            {
                adminRole = new ApplicationRole { Name = superAdminRoleName };
                await _roleManager.CreateAsync(adminRole);
            }

            var superAdminUser = await (from u in _ctx.Users
                                        join ur in _ctx.UserRoles on u.Id equals ur.UserId
                                        join r in _ctx.Roles on ur.RoleId equals r.Id
                                        where r.Name.Equals(superAdminRoleName)
                                        select u).FirstOrDefaultAsync();

            _logger.Info("Seeding super admin user");
            if (superAdminUser == null)
            {
                var userEmail = "sa@yourdomain.com";
                superAdminUser = new ApplicationUser { UserName = userEmail, Email = userEmail };
                await _userManager.CreateAsync(superAdminUser, "Sauser@123");
                await _userManager.AddToRoleAsync(superAdminUser, superAdminRoleName);
            }

            _logger.Info("Seeding data ended");
        }
    }
}
