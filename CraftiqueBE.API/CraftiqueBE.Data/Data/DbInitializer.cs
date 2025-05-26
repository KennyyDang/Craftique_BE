using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Data
{
	public static class DbInitializer
	{
		public static async Task Initialize(CraftiqueDBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			// Tạo các vai trò (Admin, Staff, Customer, Shipper)
			if (!context.Roles.Any())
			{
				var roles = new List<string> { "Admin", "Staff", "Customer", "Shipper" };
				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}
				await context.SaveChangesAsync();
			}
			;

			if (!context.Users.Any())
			{
				// Thêm người dùng
				var users = new List<(string username, string email, string name, string role)>
				{
					("admin", "admin@example.com", "Administrator", RolesHelper.Admin),
					("staff", "staff@example.com", "Store Staff", RolesHelper.Staff),
					("Kien", "kien@example.com", "Store Shipper", RolesHelper.Shipper),
					("Tai", "tai@example.com", "Store Shipper", RolesHelper.Shipper),
					("customer", "user@example.com", "Regular Customer", RolesHelper.Customer)
				};

				foreach (var (username, email, name, role) in users)
				{
					if (await userManager.FindByEmailAsync(email) == null)
					{
						var user = new User { UserName = username, Email = email, Name = name, EmailConfirmed = true };
						await userManager.CreateAsync(user, "123");
						await userManager.AddToRoleAsync(user, role);
					}
				}
				await context.SaveChangesAsync();
			}
		}
	}
}
