using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.ProductItemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IProductItemServices
	{
		Task<List<ProductItem>> GetAllWithoutFilter();
		Task<PagedResult<ProductItem>> GetAllAsync(ProductItemFilterModel filter);
		Task<ProductItem> GetByIdAsync(int id);
		Task<ProductItem> AddAsync(ProductItem productItem);
		Task<ProductItem> UpdateAsync(int id, UpdateProductItemModel newProductItem);
		Task<ProductItem> DeleteAsync(int id);
		Task<object> GetTotalSoldForProductItemAsync(int productItemId);
	}
}
