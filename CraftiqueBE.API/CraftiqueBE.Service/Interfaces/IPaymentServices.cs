using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Data.ViewModels.WalletVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IPaymentServices
	{
		Task<PaymentViewModel> CreatePaymentAsync(CreatePaymentModel model);
		Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus);
	}
}
