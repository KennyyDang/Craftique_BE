using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.Models.ProductModel;
using CraftiqueBE.Data.Models;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CraftiqueBE.Service.Services
{
	public class ProductServices : IProductServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<List<Product>> GetAllWithoutFilter()
		{
			var products = await _unitOfWork.ProductRepository.GetAllQueryable()
				.Where(r => r.IsDeleted == false)
				.Select(p => new Product
				{
					ProductID = p.ProductID,
					CategoryID = p.CategoryID,
					Name = p.Name,
					Description = p.Description,
					IsDeleted = p.IsDeleted,
					DisplayIndex = p.DisplayIndex,
					Category = p.Category,
					ProductItems = p.ProductItems.Where(pi => !pi.IsDeleted)
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
							ProductImgs = pi.ProductImgs.Where(img => !img.IsDeleted)
								.Select(img => new ProductImg
								{
									ProductImgID = img.ProductImgID,
									ProductItemID = img.ProductItemID,
									ImageUrl = img.ImageUrl,
									IsDeleted = img.IsDeleted
								}).ToList()
						}).ToList()
				})
				.AsNoTracking()
				.ToListAsync();

			return products;
		}

		public async Task<PagedResult<Product>> GetAllAsync(ProductFilterModel filter)
		{
			var query = _unitOfWork.ProductRepository.GetAllQueryable()
				.Where(r => r.IsDeleted == false);

			if (!string.IsNullOrWhiteSpace(filter.SearchName))
			{
				query = query.Where(p => p.Name.Contains(filter.SearchName));
			}

			var totalItems = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
			filter.ValidatePageNumber(totalPages);

			var items = await query
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToListAsync();

			return new PagedResult<Product>
			{
				Items = items,
				TotalItems = totalItems,
				PageNumber = filter.PageNumber,
				PageSize = filter.PageSize,
				TotalPages = totalPages
			};
		}

		public async Task<Product> GetByIdAsync(int id)
		{
			var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
			if (product == null)
				throw new KeyNotFoundException("Product not found");

			return product;
		}

		public async Task<Product> AddAsync(Product product)
		{
			await _unitOfWork.BeginTransactionAsync();

			try
			{
				var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryID);
				if (category == null)
					throw new KeyNotFoundException($"Category with ID {product.CategoryID} not found.");

				var result = await _unitOfWork.ProductRepository.AddAsync(product);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return result;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Product> UpdateAsync(int id, UpdateProductModel newProduct)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {id} not found.");

				var category = await _unitOfWork.CategoryRepository.GetByIdAsync(newProduct.CategoryID);
				if (category == null)
					throw new KeyNotFoundException($"Category with ID {newProduct.CategoryID} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				product.CategoryID = newProduct.CategoryID;
				if (!IsInvalid(newProduct.Name))
				{
					product.Name = newProduct.Name;
				}
				if (!IsInvalid(newProduct.Description))
				{
					product.Description = newProduct.Description;
				}
				if (newProduct.DisplayIndex.HasValue)
				{
					product.DisplayIndex = newProduct.DisplayIndex.Value;
				}

				var result = await _unitOfWork.ProductRepository.Update(product);

				if (!result)
				{
					await _unitOfWork.RollbackTransactionAsync();
					throw new InvalidOperationException("Failed to update product.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return product;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Product> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {id} not found.");

				var result = await _unitOfWork.ProductRepository.SoftDelete(product);
				if (!result)
				{
					await _unitOfWork.RollbackTransactionAsync();
					throw new InvalidOperationException("Failed to delete product.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return product;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}
	}
}
