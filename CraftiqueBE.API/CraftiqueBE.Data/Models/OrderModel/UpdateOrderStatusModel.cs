using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.OrderModel
{
	public class UpdateOrderStatusModel
	{
		[Required]
		public string NewStatus { get; set; }

		// Optional shipper ID for when staff assigns a shipper
		public string? ShipperId { get; set; }

		public string? RefundReason { get; set; }
	}
}
