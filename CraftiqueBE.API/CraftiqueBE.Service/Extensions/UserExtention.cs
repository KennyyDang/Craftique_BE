using CraftiqueBE.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Extensions
{
	public static class UserExtention
	{
		public static async Task<List<User>> FilterByRoleAsync(this IQueryable<User> query, string? role, UserManager<User> userManager)
		{
			if (string.IsNullOrWhiteSpace(role))
				return await query.ToListAsync();

			var users = await query.ToListAsync(); // Lấy toàn bộ user từ database
			var filteredUsers = new List<User>();

			foreach (var user in users)
			{
				var roles = await userManager.GetRolesAsync(user);
				if (roles.Contains(role))
				{
					filteredUsers.Add(user);
				}
			}

			return filteredUsers;
		}


	}
}
