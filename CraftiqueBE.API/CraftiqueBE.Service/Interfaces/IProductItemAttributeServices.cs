using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.ProductItemAttributeModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IProductItemAttributeServices
	{
		Task<PagedResult<ProductItemAttribute>> GetAllAsync(ProductItemAttributeFilterModel filter);
		Task<ProductItemAttribute> GetByIdAsync(int id);
		Task<ProductItemAttribute> AddAsync(ProductItemAttribute productItemAttribute);
		Task<ProductItemAttribute> UpdateAsync(int id, UpdateProductItemAttributeModel newProductItemAttribute);
		Task<ProductItemAttribute> DeleteAsync(int id);
	}
}
