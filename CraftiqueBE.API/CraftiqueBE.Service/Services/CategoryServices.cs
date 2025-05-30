using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.CategoryModel;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class CategoryServices : ICategoryServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CategoryServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		//get all
		public async Task<IEnumerable<Category>> GetAllAsync(string? categoryName)
		{
			var result = await _unitOfWork.CategoryRepository.GetAllAsync(categoryName == null ? null : c => c.Name == categoryName);

			result = result?.Where(r => r.IsDeleted == false) ?? new List<Category>();

			return result;
		}

		//get by id
		public async Task<Category> GetByIdAsync(int id)
		{
			var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category == null)
				throw new KeyNotFoundException("Category not found");

			return category;
		}

		public async Task<Category> AddAsync(Category category)
		{
			await _unitOfWork.BeginTransactionAsync();

			try
			{
				var result = await _unitOfWork.CategoryRepository.AddAsync(category);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return result;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		//update
		public async Task<Category> UpdateAsync(int id, UpdateCategoryModel newCategory)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
				if (category == null)
					throw new KeyNotFoundException($"Category with ID {id} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				if (!IsInvalid(newCategory.Name))
				{
					category.Name = newCategory.Name;
				}
				if (!IsInvalid(newCategory.Description))
				{
					category.Description = newCategory.Description;
				}

				var result = await _unitOfWork.CategoryRepository.Update(category);
				if (!result)
				{
					throw new InvalidOperationException("Failed to update category.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return category;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}


		//soft delete
		public async Task<Category> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
				if (category == null)
					throw new KeyNotFoundException($"Category with ID {id} not found.");

				var result = await _unitOfWork.CategoryRepository.SoftDelete(category);
				if (!result)
				{
					throw new InvalidOperationException("Failed to delete category.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return category;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

	}
}
