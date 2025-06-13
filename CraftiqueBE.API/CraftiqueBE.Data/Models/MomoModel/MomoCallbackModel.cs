using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.MomoModel
{
	public class MomoCallbackModel
	{
		public string PartnerCode { get; set; }
		public string OrderID { get; set; }
		public string RequestId { get; set; }
		public long Amount { get; set; }
		public string OrderInfo { get; set; }
		public string OrderType { get; set; }
		public int ResultCode { get; set; }
		public string Message { get; set; }
		public string PayType { get; set; }
		public string ResponseTime { get; set; }
		public string ExtraData { get; set; }
		public string Signature { get; set; }
	}
}
