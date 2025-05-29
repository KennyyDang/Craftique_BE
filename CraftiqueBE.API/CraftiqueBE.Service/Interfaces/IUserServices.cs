using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.UserModel;
using CraftiqueBE.Data.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IUserServices
	{
		Task<User> FindByEmail(string email);
		Task<User> GetByIdAsync(string id);
		Task<IEnumerable<UserViewModel>> GetAllAsync(string role);
		Task<IEnumerable<User>> GetShippersAsync();
		Task<IEnumerable<ShipperViewModel>> GetAllShippersWithPendingOrdersAsync();
		Task<(User user, string role)> GetUserWithRoleAsync(string id);
		Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser);
		Task<UserViewModel> ChangeUserRoleAsync(string userId, string newRole);
		Task<UserViewModel> IsActiveAsync(string id, bool isActive);
	}
}
