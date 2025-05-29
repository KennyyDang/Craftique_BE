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
				// 1. Tạo role nếu chưa có
				var roles = new List<string> { "Admin", "Staff", "Customer", "Shipper" };
				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}

				// 2. Tạo user mẫu
				if (!context.Users.Any())
				{
					var users = new List<(string username, string email, string name, string role)>
					{
						("admin", "admin@example.com", "Administrator", RolesHelper.Admin),
						("staff", "staff@example.com", "Store Staff", RolesHelper.Staff),
						("kien", "kien@example.com", "Store Shipper", RolesHelper.Shipper),
						("tai", "tai@example.com", "Store Shipper", RolesHelper.Shipper),
						("customer", "user@example.com", "Regular Customer", RolesHelper.Customer)
					};

					foreach (var (username, email, name, role) in users)
					{
						if (await userManager.FindByEmailAsync(email) == null)
						{
							var user = new User
							{
								UserName = username,
								Email = email,
								Name = name,
								EmailConfirmed = true
							};

							// Bạn nên dùng mật khẩu đủ mạnh nếu chưa config bỏ
							var result = await userManager.CreateAsync(user, "123");

							if (result.Succeeded)
							{
								await userManager.AddToRoleAsync(user, role);
							}
							else
							{
								// In lỗi ra console để dễ debug nếu seed thất bại
								Console.WriteLine($"❌ Failed to create user {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
							}
						}
					}
				}
			}

	}
}
