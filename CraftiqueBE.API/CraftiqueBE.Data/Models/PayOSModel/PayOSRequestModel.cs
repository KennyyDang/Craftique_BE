using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.PayOSModel
{
	public class PayOSRequestModel
	{
		public string orderCode { get; set; }
		public decimal amount { get; set; }
		public string description { get; set; }
		public string returnUrl { get; set; }
		public string cancelUrl { get; set; }
		public string signature { get; set; }
	}
}
