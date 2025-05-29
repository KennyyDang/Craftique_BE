using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.UserModel;
using CraftiqueBE.Data.ViewModels.UserVM;
using CraftiqueBE.Service.Extensions;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class UserServices : IUserServices
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UserServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
			_unitOfWork = unitOfWork;
		}

		public async Task<User> GetByIdAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null || user.IsDeleted)
				throw new KeyNotFoundException($"User with ID {id} not found.");

			return user;
		}

		public async Task<IEnumerable<User>> GetShippersAsync()
		{
			var shippers = new List<User>();

			var allUsers = await _userManager.Users
				.Where(u => !u.IsDeleted)
				.ToListAsync();

			foreach (var user in allUsers)
			{
				if (await _userManager.IsInRoleAsync(user, RolesHelper.Shipper))
				{
					shippers.Add(user);
				}
			}

			return shippers;
		}

		public async Task<IEnumerable<UserViewModel>> GetAllAsync(string role)
		{
			var usersQuery = _userManager.Users.Where(u => !u.IsDeleted);

			var users = await usersQuery.FilterByRoleAsync(role, _userManager);

			var userList = new List<UserViewModel>();

			foreach (var user in users)
			{
				var userRoles = await _userManager.GetRolesAsync(user);
				var userRole = userRoles.FirstOrDefault() ?? "No Role";

				userList.Add(new UserViewModel
				{
					Id = user.Id,
					Name = user.Name,
					Email = user.Email,
					Address = user.Address,
					PhoneNumber = user.PhoneNumber,
					Avatar = user.Avatar,
					Role = userRole
				});
			}

			return userList;
		}

		public async Task<IEnumerable<ShipperViewModel>> GetAllShippersWithPendingOrdersAsync()
		{
			var shippers = await GetShippersAsync();
			var shipperViewModels = new List<ShipperViewModel>();

			foreach (var shipper in shippers)
			{
				var pendingOrders = await _unitOfWork.OrderRepository
					.GetAllAsync(o =>
						o.ShipperID == shipper.Id &&
						o.OrderStatus != OrderStatusHelper.Completed &&
						o.OrderStatus != OrderStatusHelper.Cancelled &&
						o.OrderStatus != OrderStatusHelper.Refunded &&
						!o.IsDeleted);

				var userRoles = await _userManager.GetRolesAsync(shipper);
				var shipperViewModel = new ShipperViewModel
				{
					Id = shipper.Id,
					Name = shipper.Name,
					Email = shipper.Email,
					Address = shipper.Address,
					PhoneNumber = shipper.PhoneNumber,
					Avatar = shipper.Avatar,
					Role = userRoles.FirstOrDefault() ?? "No Role",
					PendingOrdersCount = pendingOrders.Count()
				};

				shipperViewModels.Add(shipperViewModel);
			}

			return shipperViewModels.OrderBy(s => s.PendingOrdersCount);
		}

		public async Task<(User user, string role)> GetUserWithRoleAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new KeyNotFoundException($"User with ID {id} not found.");
			}

			var userRoles = await _userManager.GetRolesAsync(user);
			var role = userRoles.FirstOrDefault() ?? "No Role";

			return (user, role);
		}

		public async Task<User> FindByEmail(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}

		public async Task<UserViewModel> UpdateAsync(string id, UserModel updatedUser)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
					throw new KeyNotFoundException($"User with ID {id} not found.");

				_mapper.Map(updatedUser, user);

				var result = await _userManager.UpdateAsync(user);
				if (!result.Succeeded)
				{
					throw new InvalidOperationException("Failed to update user.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				var userRoles = await _userManager.GetRolesAsync(user);

				var userViewModel = _mapper.Map<UserViewModel>(user);
				userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

				return userViewModel;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		//soft deleted
		public async Task<UserViewModel> IsActiveAsync(string id, bool isActive)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
					throw new KeyNotFoundException($"User with ID {id} not found.");

				IdentityResult result;
				if (isActive)
				{
					user.IsDeleted = false;
					result = await _userManager.UpdateAsync(user);
				}
				else
				{
					user.IsDeleted = true;
					result = await _userManager.UpdateAsync(user);
				}

				if (!result.Succeeded)
				{
					throw new InvalidOperationException("Failed to delete user.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				var userRoles = await _userManager.GetRolesAsync(user);

				var userViewModel = _mapper.Map<UserViewModel>(user);
				userViewModel.Role = userRoles.FirstOrDefault() ?? "No Role";

				return userViewModel;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<UserViewModel> ChangeUserRoleAsync(string userId, string newRole)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null)
					throw new KeyNotFoundException($"User with ID {userId} not found.");

				var currentRoles = await _userManager.GetRolesAsync(user);

				if (currentRoles.Contains(RolesHelper.Admin))
					throw new InvalidOperationException("Cannot change Admin role.");

				if (newRole == RolesHelper.Admin)
					throw new InvalidOperationException("Cannot assign Admin role.");

				if (!new[] { RolesHelper.Staff, RolesHelper.Shipper, RolesHelper.Customer }.Contains(newRole))
					throw new InvalidOperationException("Invalid role. Must be Staff, Shipper, or Customer.");

				var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
				if (!removeResult.Succeeded)
					throw new InvalidOperationException("Failed to remove current role.");

				var addResult = await _userManager.AddToRoleAsync(user, newRole);
				if (!addResult.Succeeded)
					throw new InvalidOperationException("Failed to add new role.");

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				var userViewModel = _mapper.Map<UserViewModel>(user);
				userViewModel.Role = newRole;

				return userViewModel;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}
	}
}
