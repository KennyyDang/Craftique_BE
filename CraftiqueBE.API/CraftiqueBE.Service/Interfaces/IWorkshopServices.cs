using CraftiqueBE.Data.Models.WorkshopRegistrationModel;
using CraftiqueBE.Data.ViewModels.WorkshopRegistrationVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IWorkshopServices
	{
		Task<List<WorkshopRegistrationViewModel>> GetAllAsync();
		Task<WorkshopRegistrationViewModel> AddAsync(CreateWorkshopRegistrationModel model);
		Task SendEmailAsync(int registrationId, string subject, string body);
		Task SendEmailBulkAsync(List<int> registrationIds, string subject, string body);
		Task<bool> ConfirmAsync(int registrationId);
	}
}
