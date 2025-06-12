using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.WalletVM
{
	public class WalletViewModel
	{
		public int WalletId { get; set; }
		public string UserId { get; set; }
		public decimal Balance { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
