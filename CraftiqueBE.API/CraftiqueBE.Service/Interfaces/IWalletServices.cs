using CraftiqueBE.Data.ViewModels.WalletVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IWalletServices
	{
		Task<WalletViewModel> GetWalletByUserIdAsync(string userId);
		Task<bool> RechargeWalletAsync(string userId, decimal amount, string description = null);
		Task<IEnumerable<WalletTransactionViewModel>> GetWalletTransactionsAsync(string userId);
	}
}
