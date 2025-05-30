using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.ProductImgModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IProductImgServices
	{
		Task<IEnumerable<ProductImg>> GetAllAsync();

		Task<List<ProductImg>> AddMultipleAsync(CreateProductImgModel model);
	}
}
