using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.MomoModel
{
	public class MomoPaymentResponseModel
	{
		public int ResultCode { get; set; }
		public string Message { get; set; }
		public string OrderID { get; set; }
		public string RequestId { get; set; }
		public string PayUrl { get; set; } // Link để redirect người dùng đến MoMo
	}
}
