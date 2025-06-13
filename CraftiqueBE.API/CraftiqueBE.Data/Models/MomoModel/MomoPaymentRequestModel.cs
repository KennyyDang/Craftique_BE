using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.MomoModel
{
	public class MomoPaymentRequestModel
	{
		public decimal Amount { get; set; }
		public string Description { get; set; }

		// Dùng mặc định nếu null
		public string? RedirectUrl { get; set; }   // Nơi chuyển hướng sau khi thanh toán
		public string? IpnUrl { get; set; }        // Nơi MoMo gọi lại khi thanh toán xong (callback)
	}
}
