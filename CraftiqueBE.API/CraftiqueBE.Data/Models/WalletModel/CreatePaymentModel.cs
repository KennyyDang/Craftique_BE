using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.WalletModel
{
	public class CreatePaymentModel
	{
		[Required]
		public string Method { get; set; }

		[Required]
		public string Status { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[Required]
		public string UserId { get; set; } // ID của người dùng thực hiện thanh toán
	}
}
