using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Data.ViewModels.CustomProductVM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface ICustomProductService
	{
		Task<CustomProductViewModel> AddCustomProductAsync(CustomProductCreateModel model);

		Task<List<CustomProductViewModel>> GetAllCustomProductsAsync();

		Task<bool> DeleteCustomProductAsync(int customProductId);
		Task<CustomProductViewModel> AddCustomProductWithImageAsync(CustomProductUploadModel model);
	}
}
