using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.Models;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftiqueBE.Service.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CraftiqueBE.Service.Services
{
	public class ProductItemServices : IProductItemServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductItemServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<List<ProductItem>> GetAllWithoutFilter()
		{
			var productItems = await _unitOfWork.ProductItemRepository.GetAllQueryable()
				.Where(r => r.IsDeleted == false)
				.Select(pi => new ProductItem
				{
					ProductItemID = pi.ProductItemID,
					ProductID = pi.ProductID,
					Name = pi.Name,
					Description = pi.Description,
					Quantity = pi.Quantity,
					DisplayIndex = pi.DisplayIndex,
					Price = pi.Price,
					IsDeleted = pi.IsDeleted,
					Product = new Product
					{
						ProductID = pi.Product.ProductID,
						CategoryID = pi.Product.CategoryID,
						Name = pi.Product.Name,
						Description = pi.Product.Description,
						IsDeleted = pi.Product.IsDeleted,
						DisplayIndex = pi.Product.DisplayIndex,
						Category = pi.Product.Category
					},
					ProductImgs = pi.ProductImgs.Where(img => !img.IsDeleted)
						.Select(img => new ProductImg
						{
							ProductImgID = img.ProductImgID,
							ProductItemID = img.ProductItemID,
							ImageUrl = img.ImageUrl,
							IsDeleted = img.IsDeleted
						}).ToList(),
					ProductItemAttributes = pi.ProductItemAttributes
						.Select(attr => new ProductItemAttribute
						{
							ProductItemAttributeID = attr.ProductItemAttributeID,
							ProductItemID = attr.ProductItemID,
							AttributeID = attr.AttributeID,
							Value = attr.Value,
							Attribute = new CraftiqueBE.Data.Entities.Attribute
							{
								AttributeID = attr.Attribute.AttributeID,
								AttributeName = attr.Attribute.AttributeName
							}
						}).ToList()
				})
				.AsNoTracking()
				.ToListAsync();

			return productItems;
		}

		public async Task<PagedResult<ProductItem>> GetAllAsync(ProductItemFilterModel filter)
		{
			var query = _unitOfWork.ProductItemRepository.GetAllQueryable()
				.ApplyBaseQuery()
				.FilterByCategory(filter.CategoryId)
				.FilterBySearchTerm(filter.SearchTerm)
				.FilterByPriceRange(filter.MinPrice, filter.MaxPrice)
				.FilterByColors(filter.Colors)
				.FilterByMaterials(filter.Materials)
				.FilterBySizes(filter.Sizes)
				.FilterByShapes(filter.Shapes)
				.ApplySorting(filter.PriceSort);

			return await query.ToPagedResultAsync(filter);
		}

		public async Task<ProductItem> GetByIdAsync(int id)
		{
			var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
				o => o.ProductImgs.Where(img => !img.IsDeleted),
				o => o.Product,
				o => o.ProductItemAttributes);

			if (productItem == null)
				throw new KeyNotFoundException("ProductItem not found");

			return productItem;
		}

		public async Task<ProductItem> AddAsync(ProductItem productItem)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var product = await _unitOfWork.ProductRepository.GetByIdAsync(productItem.ProductID);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {productItem.ProductID} not found.");

				var images = productItem.ProductImgs?.ToList() ?? new List<ProductImg>();
				productItem.ProductImgs = new List<ProductImg>();

				var result = await _unitOfWork.ProductItemRepository.AddAsync(productItem);
				await _unitOfWork.SaveChangesAsync();

				if (images.Any())
				{
					foreach (var img in images)
					{
						img.ProductItemID = result.ProductItemID;
						await _unitOfWork.ProductImgRepository.AddAsync(img);
					}
					await _unitOfWork.SaveChangesAsync();
				}

				await _unitOfWork.CommitTransactionAsync();

				return await _unitOfWork.ProductItemRepository.GetByIdAsync(result.ProductItemID,
					o => o.ProductImgs,
					o => o.Product,
					o => o.ProductItemAttributes);
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
					o => o.ProductImgs);

				if (productItem == null)
					throw new KeyNotFoundException($"ProductItem with ID {id} not found.");

				var product = await _unitOfWork.ProductRepository.GetByIdAsync(newProductItem.ProductID);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {newProductItem.ProductID} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				if (newProductItem.ProductID != 0 && newProductItem.ProductID != productItem.ProductID)
				{
					productItem.ProductID = newProductItem.ProductID;
				}

				if (!IsInvalid(newProductItem.Name))
				{
					productItem.Name = newProductItem.Name;
				}

				if (!IsInvalid(newProductItem.Description))
				{
					productItem.Description = newProductItem.Description;
				}

				if (newProductItem.Quantity != default)
				{
					productItem.Quantity = newProductItem.Quantity;
				}

				if (newProductItem.DisplayIndex != default)
				{
					productItem.DisplayIndex = newProductItem.DisplayIndex;
				}

				if (newProductItem.Price != default)
				{
					productItem.Price = newProductItem.Price;
				}

				if (newProductItem.RemoveImageIds?.Any() == true)
				{
					var imagesToRemove = productItem.ProductImgs
						.Where(img => newProductItem.RemoveImageIds.Contains(img.ProductImgID))
						.ToList();

					foreach (var img in imagesToRemove)
					{
						await _unitOfWork.ProductImgRepository.SoftDelete(img);
					}
				}

				if (newProductItem.UpdatedProductImgs?.Any() == true)
				{
					foreach (var updatedImg in newProductItem.UpdatedProductImgs)
					{
						var existingImg = productItem.ProductImgs
							.FirstOrDefault(img => img.ProductImgID == updatedImg.ProductImgID);

						if (existingImg != null && !IsInvalid(updatedImg.ImageUrl))
						{
							existingImg.ImageUrl = updatedImg.ImageUrl;
							await _unitOfWork.ProductImgRepository.Update(existingImg);
						}
					}
				}

				var result = await _unitOfWork.ProductItemRepository.Update(productItem);

				if (!result)
				{
					await _unitOfWork.RollbackTransactionAsync();
					throw new InvalidOperationException("Failed to update product item.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return await _unitOfWork.ProductItemRepository.GetByIdAsync(id,
					o => o.ProductImgs,
					o => o.Product,
					o => o.ProductItemAttributes);
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<ProductItem> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(id);
				if (productItem == null)
					throw new KeyNotFoundException($"ProductItem with ID {id} not found.");

				var result = await _unitOfWork.ProductItemRepository.SoftDelete(productItem);
				if (!result)
				{
					throw new InvalidOperationException("Failed to delete product item.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return productItem;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<object> GetTotalSoldForProductItemAsync(int productId)
		{
			// Check ProductItem trước
			var productItem = await _unitOfWork.ProductItemRepository
				.GetAllQueryable()
				.Where(pi => pi.ProductItemID == productId && !pi.IsDeleted)
				.FirstOrDefaultAsync();

			if (productItem != null)
			{
				var totalSold = await _unitOfWork.OrderRepository
					.GetAllQueryable()
					.Where(o => o.OrderStatus == OrderStatusHelper.Completed)
					.SelectMany(o => o.OrderDetails)
					.Where(od => od.ProductItemID == productId)
					.SumAsync(od => od.Quantity);

				return new
				{
					Type = "ProductItem",
					Id = productId,
					Name = productItem.Name,
					TotalSold = totalSold
				};
			}

			// Nếu không tìm thấy ProductItem → check CustomProductFile
			var customFile = await _unitOfWork.CustomProductFileRepository.GetByIdAsync(productId);
			if (customFile != null)
			{
				var customProduct = await _unitOfWork.CustomProductRepository.GetByIdAsync(customFile.CustomProductID);

				var totalSold = await _unitOfWork.OrderRepository
					.GetAllQueryable()
					.Where(o => o.OrderStatus == OrderStatusHelper.Completed)
					.SelectMany(o => o.OrderDetails)
					.Where(od => od.CustomProductFileID == productId)
					.SumAsync(od => od.Quantity);

				return new
				{
					Type = "CustomProduct",
					Id = productId,
					Name = customProduct?.CustomName,
					TotalSold = totalSold
				};
			}

			// Nếu không có ở cả 2 bảng
			throw new KeyNotFoundException($"Không tìm thấy ProductItem hoặc CustomProductFile với ID {productId}");
		}

	}
}
