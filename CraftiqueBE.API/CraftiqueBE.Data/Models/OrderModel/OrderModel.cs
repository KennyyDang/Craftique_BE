using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.OrderModel
{
	public class OrderModel
	{
		[Required(ErrorMessage = "User ID is required.")]
		public string UserID { get; set; }

		[Required(ErrorMessage = "Order date is required.")]
		public DateTime OrderDate { get; set; }

		[MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
		public string Address { get; set; }

		[MaxLength(500, ErrorMessage = "Payment method cannot exceed 500 characters.")]
		public string PaymentMethod { get; set; }

		[Required(ErrorMessage = "Shipping method is required.")]
		public int ShippingMethodID { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Total must be a positive value.")]
		public double Total { get; set; }

		[ForeignKey("Voucher")]
		public int? VoucherID { get; set; }

		public List<OrderDetailModel> OrderDetails { get; set; }
	}

	public class OrderDetailModel
	{
		public int? ProductItemID { get; set; }   // ➕ sửa thành nullable
		public int? CustomProductFileID { get; set; }  // ➕ thêm CustomProductFileID

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0.")]
		public int Quantity { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0.")]
		public double Price { get; set; }
	}

}
