using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.OrderVM
{
	public class OrderViewModel
	{
		public int OrderID { get; set; }
		public string UserID { get; set; }
		public string? ShipperID { get; set; }
		public DateTime OrderDate { get; set; }
		public string Address { get; set; }
		public string PaymentMethod { get; set; }
		public int ShippingMethodID { get; set; }
		public double Total { get; set; }
		public string OrderStatus { get; set; }
		public int? VoucherID { get; set; }
		public List<OrderDetailViewModel> OrderDetails { get; set; }
	}

	public class OrderDetailViewModel
	{
		public int OrderDetailID { get; set; }
		public int OrderID { get; set; }
		public int? ProductItemID { get; set; }
		public int? CustomProductFileID { get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }

		public string? FileUrl { get; set; }
		public string? CustomProductImageUrl { get; set; }
		public string? CustomProductName { get; set; }
	}
}
