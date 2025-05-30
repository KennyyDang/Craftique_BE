using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.ProductItemAttributeModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class ProductItemAttributeServices : IProductItemAttributeServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ProductItemAttributeServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<PagedResult<ProductItemAttribute>> GetAllAsync(ProductItemAttributeFilterModel filter)
		{
			var query = _unitOfWork.ProductItemAttributeRepository.GetAllQueryable()
				.Include(pia => pia.Attribute)
				.Where(r => r.IsDeleted == false);

			if (!string.IsNullOrWhiteSpace(filter.SearchAttributeName))
			{
				query = query.Where(pia => pia.Attribute.AttributeName.Contains(filter.SearchAttributeName));
			}

			var totalItems = await query.CountAsync();
			var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
			filter.ValidatePageNumber(totalPages);

			var items = await query
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToListAsync();

			return new PagedResult<ProductItemAttribute>
			{
				Items = items,
				TotalItems = totalItems,
				PageNumber = filter.PageNumber,
				PageSize = filter.PageSize,
				TotalPages = totalPages
			};
		}

		public async Task<ProductItemAttribute> GetByIdAsync(int id)
		{
			var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
			if (productItemAttribute == null)
				throw new KeyNotFoundException("ProductItemAttribute not found");

			return productItemAttribute;
		}

		public async Task<ProductItemAttribute> AddAsync(ProductItemAttribute productItemAttribute)
		{
			await _unitOfWork.BeginTransactionAsync();

			try
			{
				var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemAttribute.ProductItemID);
				if (productItem == null)
					throw new KeyNotFoundException($"ProductItem with ID {productItemAttribute.ProductItemID} not found.");

				var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(productItemAttribute.AttributeID);
				if (attribute == null)
					throw new KeyNotFoundException($"Attribute with ID {productItemAttribute.AttributeID} not found.");

				var result = await _unitOfWork.ProductItemAttributeRepository.AddAsync(productItemAttribute);

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

		public async Task<ProductItemAttribute> UpdateAsync(int id, UpdateProductItemAttributeModel newProductItemAttribute)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
				if (productItemAttribute == null)
					throw new KeyNotFoundException($"ProductItemAttribute with ID {id} not found.");

				var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(newProductItemAttribute.ProductItemID);
				if (productItem == null)
					throw new KeyNotFoundException($"ProductItem with ID {newProductItemAttribute.ProductItemID} not found.");

				var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(newProductItemAttribute.AttributeID);
				if (attribute == null)
					throw new KeyNotFoundException($"Attribute with ID {newProductItemAttribute.AttributeID} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				productItemAttribute.ProductItemID = newProductItemAttribute.ProductItemID;
				productItemAttribute.AttributeID = newProductItemAttribute.AttributeID;
				if (!IsInvalid(newProductItemAttribute.Value))
				{
					productItemAttribute.Value = newProductItemAttribute.Value;
				}

				var result = await _unitOfWork.ProductItemAttributeRepository.Update(productItemAttribute);

				if (!result)
				{
					await _unitOfWork.RollbackTransactionAsync();
					throw new InvalidOperationException("Failed to update product item attribute.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return productItemAttribute;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<ProductItemAttribute> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var productItemAttribute = await _unitOfWork.ProductItemAttributeRepository.GetByIdAsync(id);
				if (productItemAttribute == null)
					throw new KeyNotFoundException($"ProductItemAttribute with ID {id} not found.");

				var result = await _unitOfWork.ProductItemAttributeRepository.SoftDelete(productItemAttribute);
				if (!result)
				{
					throw new InvalidOperationException("Failed to delete product item attribute.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return productItemAttribute;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}
	}
}
