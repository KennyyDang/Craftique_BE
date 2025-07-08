using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Data.ViewModels.CustomProductVM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface ICustomProductFileService
	{
		// Customer upload file / text
		Task<CustomProductFileViewModel> UploadFileAsync(CustomProductFileUploadModel model, string userId);

		// Admin, Staff xem theo custom product
		Task<List<CustomProductFileViewModel>> GetFilesByCustomProductAsync(int customProductId);

		// Customer xem file của mình
		Task<List<CustomProductFileViewModel>> GetCustomProductFilesByUserAsync(string userId);

		// Admin hoặc user xoá file
		Task<bool> DeleteFileAsync(int customProductFileId);
	}
}
