using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.Models.ProductModel;
using CraftiqueBE.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IProductServices
	{
		Task<List<Product>> GetAllWithoutFilter();
		Task<PagedResult<Product>> GetAllAsync(ProductFilterModel filter);
		Task<Product> GetByIdAsync(int id);
		Task<Product> AddAsync(Product product);
		Task<Product> UpdateAsync(int id, UpdateProductModel newProduct);
		Task<Product> DeleteAsync(int id);
	}
}
