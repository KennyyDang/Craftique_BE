using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.PaymentVM
{
	public class TransactionViewModel
	{
		public int TransactionId { get; set; }
		public int PaymentId { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public decimal Amount { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedAt { get; set; }
	}

}
