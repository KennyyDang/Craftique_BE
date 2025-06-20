using CraftiqueBE.Data.Models.DesignUploadModel;
using CraftiqueBE.Data.ViewModels.UserDesignUploadVM;


namespace CraftiqueBE.Service.Interfaces
{
	public interface IDesignService
	{
		Task<DesignUploadViewModel> UploadDesignAsync(string userId, DesignUploadRequestModel request);
		Task<CustomizationViewModel> CreateCustomizationAsync(CreateCustomizationModel model);
	}
}
