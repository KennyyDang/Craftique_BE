using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.CategoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface ICategoryServices
	{
		Task<Category> AddAsync(Category category);
		Task<Category> DeleteAsync(int id);
		Task<IEnumerable<Category>> GetAllAsync(string? categoryName);
		Task<Category> GetByIdAsync(int id);
		Task<Category> UpdateAsync(int id, UpdateCategoryModel newCategory);
	}
}
