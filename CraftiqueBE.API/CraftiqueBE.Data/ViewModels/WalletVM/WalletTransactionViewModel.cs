using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.WalletVM
{
	public class WalletTransactionViewModel
	{
		public int TransactionId { get; set; }
		public string Type { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
