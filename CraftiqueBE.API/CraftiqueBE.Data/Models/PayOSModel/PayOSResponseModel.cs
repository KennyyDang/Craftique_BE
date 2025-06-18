using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.PayOSModel
{
	public class PayOSResponseModel
	{
		public string orderCode { get; set; }
		public string checkoutUrl { get; set; }
	}
}
