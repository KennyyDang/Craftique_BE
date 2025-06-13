using CraftiqueBE.Data.Models.MomoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IMomoService
	{
		Task<MomoPaymentResponseModel> CreatePaymentAsync(MomoPaymentRequestModel model);
	}
}
