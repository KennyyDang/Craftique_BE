using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.WalletVM
{
	public class PaymentViewModel
	{
		public int PaymentId { get; set; }
		public string UserId { get; set; }
		public string OrderId { get; set; }
		public string RequestId { get; set; }
		public string Status { get; set; }
		public decimal Amount { get; set; }
		public string PayUrl { get; set; }
		public string Provider { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? PaidAt { get; set; }
	}
}
