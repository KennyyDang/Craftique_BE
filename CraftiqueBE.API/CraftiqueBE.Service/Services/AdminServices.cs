using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.ViewModels.ProductItemVM;
using CraftiqueBE.Data.ViewModels.UserVM;
using CraftiqueBE.Service.Extensions;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class AdminServices : IAdminServices
	{
		private readonly UserManager<User> _userManager;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserServices _userServices;

		public AdminServices(UserManager<User> userManager, IUnitOfWork unitOfWork, IUserServices userServices)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_userServices = userServices;
		}

		public async Task<int> GetTotalUserAsync()
		{
			return await _userManager.Users.CountAsync();
		}

		public async Task<object> GetTotalRevenueAsync(TimeModel model)
		{
			var query = _unitOfWork.OrderRepository.GetAllQueryable()
				.FilterByYear(model.Year)
				.FilterByQuarter(model.Quarter, model.Year)
				.FilterByMonth(model.Month, model.Year)
				.FilterByDay(model.Day, model.Month, model.Year);


			double totalRevenue = await query.SumAsync(o => o.Total);

			return new
			{
				IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
				Year = model.Year,
				Quarter = model.Quarter,
				Month = model.Month,
				Day = model.Day,
				TotalRevenue = totalRevenue
			};
		}

		public async Task<object> GetTopSellingProductItemsAsync(TimeModel model, int? topN)
		{
			var query = _unitOfWork.OrderRepository.GetAllQueryable()
				//.Where(o => o.OrderStatus == "Completed")
				.FilterByYear(model.Year)
				.FilterByQuarter(model.Quarter, model.Year) // Thêm bộ lọc theo quý
				.FilterByMonth(model.Month, model.Year)
				.FilterByDay(model.Day, model.Month, model.Year);

			var topProducts = await query
				.SelectMany(o => o.OrderDetails)
				.GroupBy(od => od.ProductItemID)
				.Select(g => new
				{
					ProductItemId = g.Key,
					TotalSold = g.Sum(od => od.Quantity)
				})
				.OrderByDescending(g => g.TotalSold)
				.Take(topN ?? int.MaxValue) // topN or all
				.ToListAsync();

			var productItemIds = topProducts.Select(p => p.ProductItemId).ToList();

			var products = await _unitOfWork.ProductItemRepository
				.GetAllQueryable()
				.Where(p => productItemIds.Contains(p.ProductItemID))
				.ToListAsync();

			//last infomation
			var result = products.Select(p => new ProductItemSalesViewModel
			{
				ProductItemId = p.ProductItemID,
				Name = p.Name,
				Price = p.Price,
				TotalSold = topProducts.FirstOrDefault(tp => tp.ProductItemId == p.ProductItemID)?.TotalSold ?? 0
			})
			.OrderByDescending(p => p.TotalSold) // Sắp xếp lại từ cao xuống thấp
			.ToList();

			return new
			{
				IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
				Year = model.Year,
				Quarter = model.Quarter,
				Month = model.Month,
				Day = model.Day,
				TopProductItems = result
			};
		}

		public async Task<object> GetTopCustomersAsync(TimeModel model, int? topN)
		{
			var query = _unitOfWork.OrderRepository.GetAllQueryable()
				.FilterByYear(model.Year)
				.FilterByQuarter(model.Quarter, model.Year)
				.FilterByMonth(model.Month, model.Year)
				.FilterByDay(model.Day, model.Month, model.Year);

			var topCustomers = await query
				.GroupBy(o => o.UserID)
				.Select(g => new
				{
					UserID = g.Key,
					TotalSpent = g.Sum(o => o.Total), // Tổng số tiền đã chi tiêu
					OrderCount = g.Count() // Số lượng đơn hàng đã đặt
				})
				.OrderByDescending(g => g.TotalSpent)
				.Take(topN ?? int.MaxValue) // Nếu không truyền topN, lấy tất cả
				.ToListAsync();

			var userIds = topCustomers.Select(c => c.UserID).ToList();

			var users = await _userManager.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new
				{
					u.Id,
					u.Name,
					u.Email
				})
				.ToListAsync();

			var topCustomersWithDetails = topCustomers.Select(c => new
			{
				c.UserID,
				CustomerName = users.FirstOrDefault(u => u.Id == c.UserID)?.Name ?? "Unknown",
				CustomerEmail = users.FirstOrDefault(u => u.Id == c.UserID)?.Email,
				c.TotalSpent,
				c.OrderCount
			}).ToList();

			return new
			{
				IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue,
				Year = model.Year,
				Quarter = model.Quarter,
				Month = model.Month,
				Day = model.Day,
				TopCustomers = topCustomersWithDetails
			};
		}

		public async Task<object> GetTotalProductItemsAsync(CategoryProductFilterModel filter)
		{
			// Get all product items with details
			var productItemsQuery = _unitOfWork.ProductItemRepository.GetAllQueryable()
				.ApplyBaseQuery()
				.Where(pi => !pi.IsDeleted);

			// Apply filters if provided
			if (filter.CategoryId.HasValue)
			{
				productItemsQuery = productItemsQuery.FilterByCategory(filter.CategoryId);
			}

			if (filter.ProductId.HasValue)
			{
				productItemsQuery = productItemsQuery.FilterByProduct(filter.ProductId);
			}

			// Get total item count
			var totalItems = await productItemsQuery.CountAsync();

			// Get individual product item counts
			var productItemDetails = await productItemsQuery
				.Select(pi => new
				{
					ProductItemId = pi.ProductItemID,
					ProductItemName = pi.Name,
					ProductId = pi.ProductID,
					ProductName = pi.Product.Name,
					CategoryId = pi.Product.CategoryID,
					CategoryName = pi.Product.Category.Name,
					Quantity = pi.Quantity,
					Price = pi.Price
				})
				.ToListAsync();

			return new
			{
				TotalCount = totalItems,
				ProductItems = productItemDetails
			};
		}

		public async Task<IEnumerable<ShipperViewModel>> GetAllShippersWithPendingOrdersAsync()
		{
			return await _userServices.GetAllShippersWithPendingOrdersAsync();
		}
	}
}
